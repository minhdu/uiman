namespace UnuGames.MVVM
{
	public interface IModule
	{
		object OriginalData {
			get;
			set;
		}

		ViewModelBehaviour VM { get; }
	}
}
