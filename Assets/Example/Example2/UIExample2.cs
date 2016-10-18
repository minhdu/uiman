using UnityEngine;

using UnuGames;
using UnuGames.MVVM;

// This code is generated automatically by UIMan ViewModelGerenrator, please do not modify!

public partial class UIExample2 : UIManDialog {


	string _messsage = "";
	[UIManProperty]
	public string Messsage {
		get { return _messsage; }
		set { _messsage = value; OnPropertyChanged(); }
	}

	string _buttonText = "";
	[UIManProperty]
	public string ButtonText {
		get { return _buttonText; }
		set { _buttonText = value; OnPropertyChanged(); }
	}

	Color _textColor;
	[UIManProperty]
	public Color TextColor {
		get { return _textColor; }
		set { _textColor = value; OnPropertyChanged(); }
	}

}
