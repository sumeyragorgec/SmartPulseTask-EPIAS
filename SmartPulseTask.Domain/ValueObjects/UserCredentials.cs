namespace SmartPulseTask.Domain.ValueObjects;
public class UserCredentials
{
    public string Username { get; private set; }
    public string Password { get; private set; }

    public UserCredentials(string username, string password)
    {
        if (string.IsNullOrWhiteSpace(username))
            throw new ArgumentException("Username cannot be null or empty", nameof(username));

        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Password cannot be null or empty", nameof(password));

        Username = username;
        Password = password;
    }

    public override bool Equals(object? obj)
    {
        if (obj is not UserCredentials other) return false;
        return Username == other.Username && Password == other.Password;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Username, Password);
    }
}
