using EduHub.Ch02.Domain;

namespace EduHub.Ch02.Repositories;

/// <summary>Kho dữ liệu CLB — interface nhỏ (ISP), chỉ việc liên quan CLB.</summary>
public interface IClubRepository
{
    IReadOnlyList<Club> GetAll();
    Club? GetById(Guid id);
    void Add(Club club);
}
