using System;
using UIKit;
using System.Threading.Tasks;

namespace Chance.MvvmCross.Plugins.UserInteraction.Touch
{
	public class UserInteraction : IUserInteraction
	{
		const string OK = "OK";
		const string YES = "Да";
		const string NO = "Нет";
		const string CANCEL = "Отмена";
		const string MAYBE = "Возможно";

		public void Confirm(string message, Action okClicked, string title = null)
		{
			Confirm(message, confirmed =>
			{
				if (confirmed)
					okClicked();
			},
			title);
		}

		public void Confirm(string message, Action<bool> answer, string title = null)
		{
			UIApplication.SharedApplication.InvokeOnMainThread(() =>
			{
				var confirm = new UIAlertView(title ?? string.Empty, message, null, CANCEL, OK);
				if (answer != null)
				{
					confirm.Clicked +=
						(sender, args) =>
							answer(confirm.CancelButtonIndex != args.ButtonIndex);
				}
				confirm.Show();
			});
		}

		public Task<bool> ConfirmAsync(string message, string title = null)
		{
			var tcs = new TaskCompletionSource<bool>();
			Confirm(message, (r) => tcs.TrySetResult(r), title);
			return tcs.Task;
        }

        public void ConfirmThreeButtons(string message, Action<ConfirmThreeButtonsResponse> answer, string title = null)
        {
			var confirm = new UIAlertView(title ?? string.Empty, message, null, CANCEL, OK, MAYBE);
            if (answer != null)
            {
                confirm.Clicked +=
                    (sender, args) =>
                    {
                        var buttonIndex = args.ButtonIndex;
                        if (buttonIndex == confirm.CancelButtonIndex)
                            answer(ConfirmThreeButtonsResponse.Negative);
                        else if (buttonIndex == confirm.FirstOtherButtonIndex)
                            answer(ConfirmThreeButtonsResponse.Positive);
                        else
                            answer(ConfirmThreeButtonsResponse.Neutral);
                    };
                confirm.Show();
            }
        }

        public Task<ConfirmThreeButtonsResponse> ConfirmThreeButtonsAsync(string message, string title = null)
        {
            var tcs = new TaskCompletionSource<ConfirmThreeButtonsResponse>();
			ConfirmThreeButtons(message, (r) => tcs.TrySetResult(r), title);
            return tcs.Task;
        }

		public void Alert(string message, Action done = null, string title = null)
		{
			UIApplication.SharedApplication.InvokeOnMainThread(() =>
			{
				var alert = new UIAlertView(title ?? string.Empty, message, null, OK);
				if (done != null)
				{
					alert.Clicked += (sender, args) => done();
				}
				alert.Show();
			});

		}

		public Task AlertAsync(string message, string title = null)
		{
			var tcs = new TaskCompletionSource<object>();
			Alert(message, () => tcs.TrySetResult(null), title);
			return tcs.Task;
        }

		public void Input(string message, Action<string> okClicked, string placeholder = null, string title = null, string initialText = null, bool itsPassword = false)
		{
			Input(message, (ok, text) =>
			{
				if (ok)
					okClicked(text);
			},
	      	placeholder, title, initialText);
		}

		public void Input(string message, Action<bool, string> answer, string placeholder = null, string title = null, string initialText = null, bool itsPassword = false)
		{
			UIApplication.SharedApplication.InvokeOnMainThread(() =>
			{
				var input = new UIAlertView(title ?? string.Empty, message, null, CANCEL, OK);
				input.AlertViewStyle = itsPassword ? UIAlertViewStyle.SecureTextInput : UIAlertViewStyle.PlainTextInput;
				var textField = input.GetTextField(0);
				textField.Placeholder = placeholder;
				textField.Text = initialText;
				if (answer != null)
				{
					input.Clicked +=
						(sender, args) =>
							answer(input.CancelButtonIndex != args.ButtonIndex, textField.Text);
				}
				input.Show();
			});
		}

		public Task<InputResponse> InputAsync(string message, string placeholder = null, string title = null, string initialText = null, bool itsPassword = false)
		{
			var tcs = new TaskCompletionSource<InputResponse>();
			Input(message, (ok, text) => tcs.TrySetResult(new InputResponse {Ok = ok, Text = text}), placeholder, title, initialText);
			return tcs.Task;
		}
	}
}

