using System.Data;

namespace Model.Data;

public interface IDataExecutionContext
{
	Task ExecuteWithTransactionAsync(
		Func<IRepositoryRegistry, Task> action,
		IsolationLevel isolationLevel = IsolationLevel.Snapshot,
		CancellationToken cancellationToken = default);

	Task<TResponse> ExecuteWithTransactionAsync<TResponse>(
		Func<IRepositoryRegistry, Task<TResponse>> func,
		IsolationLevel isolationLevel = IsolationLevel.Snapshot,
		CancellationToken cancellationToken = default);
    
	Task ExecuteAsync(
		Func<IRepositoryRegistry, Task> action,
		CancellationToken cancellationToken = default);

	Task<TResponse> ExecuteAsync<TResponse>(
		Func<IRepositoryRegistry, Task<TResponse>> func,
		CancellationToken cancellationToken = default);
}