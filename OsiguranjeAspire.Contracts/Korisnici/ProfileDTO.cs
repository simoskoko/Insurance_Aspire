namespace OsiguranjeAspire.Contracts.Korisnici;

public sealed record ProfileDTO(
    string Name,
    string LastName,
    int UserId,
    DateOnly? DateOfBirth,
    decimal TotalMoneyEarned);
