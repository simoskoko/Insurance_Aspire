namespace OsiguranjeAspire.Contracts.Korisnici;

public sealed record AdminProfileSearchDTO(
    int UserId,
    string Username,
    string Name,
    string LastName,
    string Jmbg,
    DateOnly? DateOfBirth,
    int RoleId,
    int Lob);
