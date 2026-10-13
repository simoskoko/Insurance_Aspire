namespace OsiguranjeAspire.Contracts.Korisnici;

public sealed record ProfileDTO(
    string Name,
    string LastName,
    int UserId,
    string Jmbg,
    DateOnly? DateOfBirth,
    decimal TotalMoneyEarned);
