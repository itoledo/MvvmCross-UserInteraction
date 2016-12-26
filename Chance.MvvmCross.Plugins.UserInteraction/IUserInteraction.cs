using System;
using System.Threading.Tasks;

namespace Chance.MvvmCross.Plugins.UserInteraction
{
	public interface IUserInteraction
	{
		void Confirm(string message, Action okClicked, string title = null);
		void Confirm(string message, Action<bool> answer, string title = null);
		Task<bool> ConfirmAsync(string message, string title = null);

		void Alert(string message, Action done = null, string title = null);
		Task AlertAsync(string message, string title = null);

		void Input(string message, Action<string> okClicked, string placeholder = null, string title = null, string initialText = null, bool itsPassword = false);
		void Input(string message, Action<bool, string> answer, string placeholder = null, string title = null, string initialText = null, bool itsPassword = false);
		Task<InputResponse> InputAsync(string message, string placeholder = null, string title = null, string initialText = null, bool itsPassword = false);

	    void ConfirmThreeButtons(string message, Action<ConfirmThreeButtonsResponse> answer, string title = null);
	    Task<ConfirmThreeButtonsResponse> ConfirmThreeButtonsAsync(string message, string title = null);
	}
}

