using UnityEngine;
using System.Collections;
using UnuGames;
using UnuGames.MVVM;

[System.Serializable]
public partial class MissionModel : ObservableModel {

	int _level;
	[UIManProperty]
	public int Level {
		get { return _level; }
		set { _level = value; OnPropertyChanged(); }
	}

	int _star;
	[UIManProperty]
	public int Star {
		get { return _star; }
		set { _star = value; OnPropertyChanged(); }
	}

	bool _locked;
	[UIManProperty]
	public bool Locked {
		get { return _locked; }
		set { _locked = value; OnPropertyChanged(); }
	}
}
