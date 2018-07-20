
using UnuGames;
using UnuGames.MVVM;

// This code is generated automatically by UIMan - UI Generator, please do not modify!

public partial class UserModel : ObservableModel {


	string _avatar = "";
	[UIManProperty]
	public string Avatar {
		get { return _avatar; }
		set { _avatar = value; OnPropertyChanged(); }
	}

	string _name = "";
	[UIManProperty]
	public string Name {
		get { return _name; }
		set { _name = value; OnPropertyChanged(); }
	}

	int _level = 0;
	[UIManProperty]
	public int Level {
		get { return _level; }
		set { _level = value; OnPropertyChanged(); }
	}

	int _rank = 0;
	[UIManProperty]
	public int Rank {
		get { return _rank; }
		set { _rank = value; OnPropertyChanged(); }
	}

}
