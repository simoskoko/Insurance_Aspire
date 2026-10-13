namespace OsiguranjeAspire.Contracts.Korisnici;

public sealed record ProfileUpdateDTO(
    string Name,
    string LastName,
    string Jmbg,
    DateOnly? DateOfBirth);