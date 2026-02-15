using FluentAssertions;
using Moq;
using OrderManagementSystem.Authentication;
using OrderManagementSystem.Contracts;
using OrderManagementSystem.Entity;
using OrderManagementSystem.Service;

namespace OrderManagementSystem.Tests;

public class AuthServiceTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IJwtProvider> _jwtProviderMock;
    private readonly AuthService _sut;

    public AuthServiceTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _jwtProviderMock = new Mock<IJwtProvider>();
        _sut = new AuthService(_userRepositoryMock.Object, _jwtProviderMock.Object);
    }

    [Fact]
    public async Task RegisterAsync_WhenUsernameAlreadyExists_ShouldReturnFailure()
    {
        // Arrange
        var request = new UserRegisterRequest("existing@example.com", "Password123!", "Customer");
        
        var existingUser = new User
        {
            UserId = 1,
            Username = "existing@example.com",
            PasswordHash = "hashedPassword",
            Role = "Customer"
        };

        _userRepositoryMock
            .Setup(x => x.GetByUsernameAsync("existing@example.com"))
            .ReturnsAsync(existingUser);

        // Act
        var result = await _sut.RegisterAsync(request);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("User.Exists");
    }

    [Fact]
    public async Task RegisterAsync_WhenUsernameIsAvailable_ShouldCreateUser()
    {
        // Arrange
        var request = new UserRegisterRequest("newuser@example.com", "Password123!", "Customer");

        _userRepositoryMock
            .Setup(x => x.GetByUsernameAsync("newuser@example.com"))
            .ReturnsAsync((User?)null);

        _userRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<User>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.RegisterAsync(request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Username.Should().Be("newuser@example.com");
        _userRepositoryMock.Verify(x => x.AddAsync(It.IsAny<User>()), Times.Once);
    }

    [Fact]
    public async Task RegisterAsync_WhenSuccessful_ShouldHashPassword()
    {
        // Arrange
        var request = new UserRegisterRequest("newuser@example.com", "PlainPassword123!", "Customer");

        _userRepositoryMock
            .Setup(x => x.GetByUsernameAsync("newuser@example.com"))
            .ReturnsAsync((User?)null);

        User? capturedUser = null;
        _userRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<User>()))
            .Callback<User>(user => capturedUser = user)
            .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.RegisterAsync(request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        capturedUser.Should().NotBeNull();
        capturedUser!.PasswordHash.Should().NotBe("PlainPassword123!");
        capturedUser.PasswordHash.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task RegisterAsync_ShouldSetCorrectRole()
    {
        // Arrange
        var request = new UserRegisterRequest("admin@example.com", "Password123!", "Admin");

        _userRepositoryMock
            .Setup(x => x.GetByUsernameAsync("admin@example.com"))
            .ReturnsAsync((User?)null);

        User? capturedUser = null;
        _userRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<User>()))
            .Callback<User>(user => capturedUser = user)
            .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.RegisterAsync(request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        capturedUser.Should().NotBeNull();
        capturedUser!.Role.Should().Be("Admin");
    }

    [Fact]
    public async Task LoginAsync_WhenUserDoesNotExist_ShouldReturnFailure()
    {
        // Arrange
        var request = new UserLoginRequest("nonexistent@example.com", "Password123!");

        _userRepositoryMock
            .Setup(x => x.GetByUsernameAsync("nonexistent@example.com"))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _sut.LoginAsync(request);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Auth.Invalid");
    }

    [Fact]
    public async Task LoginAsync_WhenPasswordIsIncorrect_ShouldReturnFailure()
    {
        // Arrange
        var user = new User
        {
            UserId = 1,
            Username = "user@example.com",
            Role = "Customer"
        };
        
        // Hash a different password
        var passwordHasher = new Microsoft.AspNetCore.Identity.PasswordHasher<User>();
        user.PasswordHash = passwordHasher.HashPassword(user, "CorrectPassword123!");

        var request = new UserLoginRequest("user@example.com", "WrongPassword123!");

        _userRepositoryMock
            .Setup(x => x.GetByUsernameAsync("user@example.com"))
            .ReturnsAsync(user);

        // Act
        var result = await _sut.LoginAsync(request);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Auth.Invalid");
    }

    [Fact]
    public async Task LoginAsync_WhenCredentialsAreValid_ShouldReturnTokenAndUserInfo()
    {
        // Arrange
        var user = new User
        {
            UserId = 1,
            Username = "user@example.com",
            Role = "Customer"
        };
        
        var passwordHasher = new Microsoft.AspNetCore.Identity.PasswordHasher<User>();
        user.PasswordHash = passwordHasher.HashPassword(user, "Password123!");

        var request = new UserLoginRequest("user@example.com", "Password123!");

        _userRepositoryMock
            .Setup(x => x.GetByUsernameAsync("user@example.com"))
            .ReturnsAsync(user);

        _jwtProviderMock
            .Setup(x => x.Generate(user))
            .Returns("fake-jwt-token");

        // Act
        var result = await _sut.LoginAsync(request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Token.Should().Be("fake-jwt-token");
        result.Value.Role.Should().Be("Customer");
        result.Value.UserId.Should().Be(1);
    }

    [Fact]
    public async Task LoginAsync_WhenSuccessful_ShouldGenerateJwtToken()
    {
        // Arrange
        var user = new User
        {
            UserId = 1,
            Username = "user@example.com",
            Role = "Admin"
        };
        
        var passwordHasher = new Microsoft.AspNetCore.Identity.PasswordHasher<User>();
        user.PasswordHash = passwordHasher.HashPassword(user, "AdminPassword123!");

        var request = new UserLoginRequest("user@example.com", "AdminPassword123!");

        _userRepositoryMock
            .Setup(x => x.GetByUsernameAsync("user@example.com"))
            .ReturnsAsync(user);

        _jwtProviderMock
            .Setup(x => x.Generate(user))
            .Returns("jwt-token-for-admin");

        // Act
        var result = await _sut.LoginAsync(request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _jwtProviderMock.Verify(x => x.Generate(user), Times.Once);
    }
}
