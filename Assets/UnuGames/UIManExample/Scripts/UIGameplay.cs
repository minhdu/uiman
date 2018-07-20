
using UnuGames;
using UnuGames.MVVM;

// This code is generated automatically by UIMan - UI Generator, please do not modify!

public partial class UIGameplay : UIManScreen {


	float _circleProgress = 0;
	[UIManProperty]
	public float CircleProgress {
		get { return _circleProgress; }
		set { _circleProgress = value; OnPropertyChanged(); }
	}

	float _progress = 0;
	[UIManProperty]
	public float Progress {
		get { return _progress; }
		set { _progress = value; OnPropertyChanged(); }
	}

	int _timer = 0;
	[UIManProperty]
	public int Timer {
		get { return _timer; }
		set { _timer = value; OnPropertyChanged(); }
	}

}
