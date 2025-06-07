using Model.Domain;

namespace Model.Data.Repositories;

public class UserProfileRepository(DataContext context) : RepositoryBase<DataContext, UserProfile, int>(context), IUserProfileRepository;