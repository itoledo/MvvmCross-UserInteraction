using System;
using Android.App;
using Android.Widget;
using System.Threading.Tasks;
using MvvmCross.Platform;
using MvvmCross.Platform.Droid.Platform;
using AlertDialog = Android.Support.V7.App.AlertDialog;

namespace Chance.MvvmCross.Plugins.UserInteraction.Droid
{
	public class UserInteraction : IUserInteraction
	{
		const string OK = "OK";
		const string YES = "Да";
		const string NO = "Нет";
		const string CANCEL = "Отмена";
		const string MAYBE = "Возможно";

		protected Activity CurrentActivity {
			get { return Mvx.Resolve<IMvxAndroidCurrentTopActivity>().Activity; }
		}

		public void Confirm(string message, Action okClicked, string title = null)
		{
			Confirm(message, confirmed => {
				if (confirmed)
					okClicked();
			}, title);
		}

		public void Confirm(string message, Action<bool> answer, string title = null)
		{
			Application.SynchronizationContext.Post(ignored => {
				if (CurrentActivity == null) return;
				new AlertDialog.Builder(CurrentActivity)
					.SetMessage(message)
						.SetCancelable(false)
						.SetTitle(title)
		               	.SetPositiveButton(OK, delegate {
							if (answer != null)
								answer(true);
						})
		               .SetNegativeButton(CANCEL, delegate {	
							if (answer != null)
								answer(false);
						})
						.Show();
			}, null);
		}

		public Task<bool> ConfirmAsync(string message, string title = null)
		{
			var tcs = new TaskCompletionSource<bool>();
			Confirm(message, tcs.SetResult, title);
			return tcs.Task;
		}

		public void ConfirmThreeButtons(string message, Action<ConfirmThreeButtonsResponse> answer, string title = null)
	    {
	        Application.SynchronizationContext.Post(ignored =>
            {
                if (CurrentActivity == null) return;
                new AlertDialog.Builder(CurrentActivity)
                    .SetMessage(message)
                        .SetCancelable(false)
                        .SetTitle(title)
		               	.SetPositiveButton(YES, delegate {
                            if (answer != null)
                                answer(ConfirmThreeButtonsResponse.Positive);
                        })
		               	.SetNegativeButton(NO, delegate {
                            if (answer != null)
                                answer(ConfirmThreeButtonsResponse.Negative);
                        })
		               	.SetNeutralButton(MAYBE, delegate {
                            if (answer != null)
                                answer(ConfirmThreeButtonsResponse.Neutral);
                        })
                        .Show();
            }, null);
	    }

		public Task<ConfirmThreeButtonsResponse> ConfirmThreeButtonsAsync(string message, string title = null)
	    {
	        var tcs = new TaskCompletionSource<ConfirmThreeButtonsResponse>();
	        ConfirmThreeButtons(message, tcs.SetResult, title);
	        return tcs.Task;
	    }

		public void Alert(string message, Action done = null, string title = null)
		{
			Application.SynchronizationContext.Post(ignored => {
				if (CurrentActivity == null) return;
				new AlertDialog.Builder(CurrentActivity)
					.SetMessage(message)
						.SetCancelable(false)
						.SetTitle(title)
		               	.SetPositiveButton(OK, delegate {
							if (done != null)
								done();
						})
						.Show();
			}, null);
		}

		public Task AlertAsync(string message, string title = null)
		{
			var tcs = new TaskCompletionSource<object>();
			Alert(message, () => tcs.SetResult(null), title);
			return tcs.Task;
		}

		public void Input(string message, Action<string> okClicked, string placeholder = null, string title = null, string initialText = null)
		{
			Input(message, (ok, text) => {
				if (ok)
					okClicked(text);
			},
	      	placeholder, title, initialText);
		}

		public void Input(string message, Action<bool, string> answer, string hint = null, string title = null, string initialText = null)
		{
			Application.SynchronizationContext.Post(ignored => {
				if (CurrentActivity == null) return;
				var input = new EditText(CurrentActivity) { Hint = hint, Text = initialText };
				input.SetSingleLine(true);

				new AlertDialog.Builder(CurrentActivity)
					.SetMessage(message)
						.SetCancelable(false)
						.SetTitle(title)
						.SetView(input)
		               	.SetPositiveButton(OK, delegate {
							if (answer != null)
								answer(true, input.Text);
						})
		               	.SetNegativeButton(CANCEL, delegate {	
							if (answer != null)
								answer(false, input.Text);
						})
						.Show();
			}, null);
		}

		public Task<InputResponse> InputAsync(string message, string placeholder = null, string title = null, string initialText = null)
		{
			var tcs = new TaskCompletionSource<InputResponse>();
			Input(message, (ok, text) => tcs.SetResult(new InputResponse {Ok = ok, Text = text}),	placeholder, title, initialText);
			return tcs.Task;
		}
	}
}

