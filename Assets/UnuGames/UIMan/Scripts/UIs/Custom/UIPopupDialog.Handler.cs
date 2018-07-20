using UnityEngine;
using UnuGames;

[UIDescriptor("Dialogs/")]
public partial class UIPopupDialog : UIManDialog {

	object[] _args;
	public override void OnShow(params object[] args)
	{
		base.OnShow(args);
		
		if(args != null)
		{
			Title = (string)args [0];
			Content = (string)args [1];
			LabelButtonYes = (string)args [2];
			if (args.Length == 4) {
				IsConfirmDialog = false;
				_args = (object[])args[3];
			} else if (args.Length == 5) {
				IsConfirmDialog = true;
				LabelButtonNo = (string)args [3];
				_args = (object[])args[4];
			}
		}
	}
	
	public void OK ()
	{
		this.Callback(0, _args);
	}

	public void No ()
	{
		this.Callback(1, _args);
	}
}
