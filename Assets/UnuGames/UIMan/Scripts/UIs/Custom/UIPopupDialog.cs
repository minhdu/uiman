
using UnuGames;
using UnuGames.MVVM;

// This code is generated automatically by UIMan ViewModelGerenrator, please do not modify!

public partial class UIPopupDialog : UIManDialog {


	string _title = "";
	[UIManProperty]
	public string Title {
		get { return _title; }
		set { _title = value; OnPropertyChanged(); }
	}

	string _content = "";
	[UIManProperty]
	public string Content {
		get { return _content; }
		set { _content = value; OnPropertyChanged(); }
	}

	string _labelButtonYes = "";
	[UIManProperty]
	public string LabelButtonYes {
		get { return _labelButtonYes; }
		set { _labelButtonYes = value; OnPropertyChanged(); }
	}

	string _labelButtonNo = "";
	[UIManProperty]
	public string LabelButtonNo {
		get { return _labelButtonNo; }
		set { _labelButtonNo = value; OnPropertyChanged(); }
	}

	bool _isConfirmDialog = false;
	[UIManProperty]
	public bool IsConfirmDialog {
		get { return _isConfirmDialog; }
		set { _isConfirmDialog = value; OnPropertyChanged(); }
	}

}
