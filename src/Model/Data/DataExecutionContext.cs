using System.Data;
using Microsoft.EntityFrameworkCore;

namespace Model.Data;

public class DataExecutionContext(
	DataContext dataContext,
	IRepositoryRegistry repositoryRegistry)
	: IDataExecutionContext
{
	public async Task ExecuteWithTransactionAsync(
		Func<IRepositoryRegistry, Task> action,
		IsolationLevel isolationLevel = IsolationLevel.Snapshot,
		CancellationToken cancellationToken = default)
	{
		var executionStrategy = dataContext.Database.CreateExecutionStrategy();

		await executionStrategy.ExecuteAsync(async () =>
		{
			await using var transaction = await dataContext.Database.BeginTransactionAsync(
				isolationLevel,
				cancellationToken);

			await action(repositoryRegistry);
			await transaction.CommitAsync(cancellationToken);
		});
	}

	public async Task<TResponse> ExecuteWithTransactionAsync<TResponse>(
		Func<IRepositoryRegistry, Task<TResponse>> func,
		IsolationLevel isolationLevel = IsolationLevel.Snapshot,
		CancellationToken cancellationToken = default)
	{
		var executionStrategy = dataContext.Database.CreateExecutionStrategy();

		var response = await executionStrategy.ExecuteAsync(async () =>
		{
			await using var transaction = await dataContext.Database.BeginTransactionAsync(
				isolationLevel,
				cancellationToken);

			var response = await func(repositoryRegistry);
			await transaction.CommitAsync(cancellationToken);
			return response;
		});

		return response;
	}

	public async Task ExecuteAsync(
		Func<IRepositoryRegistry, Task> action,
		CancellationToken cancellationToken = default)
	{
		await action(repositoryRegistry);
	}

	public async Task<TResponse> ExecuteAsync<TResponse>(
		Func<IRepositoryRegistry, Task<TResponse>> func,
		CancellationToken cancellationToken = default)
	{
		var response = await func(repositoryRegistry);
		return response;
	}
}