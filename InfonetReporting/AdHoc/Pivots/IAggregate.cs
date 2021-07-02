namespace Infonet.Reporting.AdHoc.Pivots {
	public interface IAggregate<in TInput, out TResult> {
		TResult Result { get; }
		void Ingest(TInput input);
	}
}