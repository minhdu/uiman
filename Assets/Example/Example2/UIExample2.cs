using UnityEngine;

using UnuGames;
using UnuGames.MVVM;

// This code is generated automatically by UIMan - UI Generator, please do not modify!

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

	string _internalImageName = "";
	[UIManProperty]
	public string InternalImageName {
		get { return _internalImageName; }
		set { _internalImageName = value; OnPropertyChanged(); }
	}

	bool _showImageLeft = false;
	[UIManProperty]
	public bool ShowImageLeft {
		get { return _showImageLeft; }
		set { _showImageLeft = value; OnPropertyChanged(); }
	}

	int _numberValue = 0;
	[UIManProperty]
	public int NumberValue {
		get { return _numberValue; }
		set { _numberValue = value; OnPropertyChanged(); }
	}

	string _externalImageName = "";
	[UIManProperty]
	public string ExternalImageName {
		get { return _externalImageName; }
		set { _externalImageName = value; OnPropertyChanged(); }
	}

}
