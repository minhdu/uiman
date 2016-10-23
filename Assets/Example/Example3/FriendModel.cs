
using UnuGames;
using UnuGames.MVVM;

// This code is generated automatically by UIMan - UI Generator, please do not modify!

public partial class FriendModel : ObservableModel {


	string _name = "";
	[UIManProperty]
	public string Name {
		get { return _name; }
		set { _name = value; OnPropertyChanged(); }
	}

	int _age = 0;
	[UIManProperty]
	public int Age {
		get { return _age; }
		set { _age = value; OnPropertyChanged(); }
	}

}
