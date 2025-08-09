using FluentAssertions;
using PhoneRegistry.Domain.Entities;
using PhoneRegistry.Domain.ValueObjects;

namespace PhoneRegistry.Tests;

public class BasicTests
{
    [Fact]
    public void Person_Creation_Should_Work()
    {
        // Arrange & Act
        var person = new Person("John", "Doe", "Test Company");

        // Assert
        person.FirstName.Should().Be("John");
        person.LastName.Should().Be("Doe");
        person.Company.Should().Be("Test Company");
        person.IsDeleted.Should().BeFalse();
    }

    [Fact]
    public void Person_AddContactInfo_Should_Work()
    {
        // Arrange
        var person = new Person("John", "Doe");

        // Act
        var contactInfo = person.AddContactInfo(ContactType.PhoneNumber, "1234567890");

        // Assert
        person.ContactInfos.Should().HaveCount(1);
        contactInfo.Type.Should().Be(ContactType.PhoneNumber);
        contactInfo.Content.Should().Be("1234567890");
    }

    [Fact]
    public void Person_SoftDelete_Should_Work()
    {
        // Arrange
        var person = new Person("John", "Doe");
        person.AddContactInfo(ContactType.PhoneNumber, "1234567890");

        // Act
        person.SoftDelete();

        // Assert
        person.IsDeleted.Should().BeTrue();
        person.ContactInfos.First().IsDeleted.Should().BeTrue();
    }

    [Fact]
    public void ContactInfo_Creation_Should_Work()
    {
        // Arrange & Act
        var contactInfo = new ContactInfo(Guid.NewGuid(), ContactType.EmailAddress, "test@example.com");

        // Assert
        contactInfo.Type.Should().Be(ContactType.EmailAddress);
        contactInfo.Content.Should().Be("test@example.com");
        contactInfo.IsDeleted.Should().BeFalse();
    }

    [Fact]
    public void ContactInfo_SoftDelete_Should_Work()
    {
        // Arrange
        var contactInfo = new ContactInfo(Guid.NewGuid(), ContactType.PhoneNumber, "1234567890");

        // Act
        contactInfo.SoftDelete();

        // Assert
        contactInfo.IsDeleted.Should().BeTrue();
    }
}
