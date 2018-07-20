
using UnuGames;
using UnuGames.MVVM;

// This code is generated automatically by UIMan - UI Generator, please do not modify!

public partial class PlayerModel : ObservableModel {

	int _gold = 0;
	[UIManProperty]
	public int Gold {
		get { return _gold; }
		set { _gold = value; OnPropertyChanged(); }
	}

}
