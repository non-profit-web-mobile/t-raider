using Model.Data.Repositories;

namespace Model.Data;

public interface IRepositoryRegistry
{
	IUserProfileRepository UserProfileRepository { get; }
}