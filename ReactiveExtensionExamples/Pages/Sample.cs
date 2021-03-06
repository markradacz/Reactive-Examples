﻿using System;
using System.Linq;
using Xamarin.Forms;
using System.Reactive.Linq;
using System.Reactive.Concurrency;
using ReactiveUI;
using ReactiveExtensionExamples.Utilities;
using System.Reactive;

namespace ReactiveExtensionExamples.Pages
{
	public class Sample : PageBase
	{
		Entry textEntry;
		WebView webView;

		IObservable<string>
			textEntryObservable;

		protected override void SetupUserInterface ()
		{
			Title = "Rx - Sample";

			Content = new StackLayout { 
				Padding = new Thickness(8d),
				Children = {
					(textEntry = new Entry{ Placeholder = "Enter Search Terms" }),
					(webView = new WebView {
						VerticalOptions = LayoutOptions.FillAndExpand, 
						HorizontalOptions = LayoutOptions.FillAndExpand,
					})
				}
			};
		}

		protected override void SetupReactiveObservables ()
		{
			textEntryObservable =
				Observable
					.FromEventPattern<EventHandler<TextChangedEventArgs>, TextChangedEventArgs> (
						x => textEntry.TextChanged += x, 
						x => textEntry.TextChanged -= x
					)
					.Sample (TimeSpan.FromSeconds (3), TaskPoolScheduler.Default)
					.Select(args => 
						string.Format("https://frinkiac.com/?q={0}", args.EventArgs.NewTextValue.Replace(" ", "+"))
					);
		}

		protected override void SetupReactiveSubscriptions ()
		{
			textEntryObservable
				.Subscribe (searchUrl => {
					Device.BeginInvokeOnMainThread(() => {
						try {
							webView.Source = searchUrl;
						} catch {
							webView.Source = "https://frinkiac.com/caption/S04E05/1135500";
						}
					});
				})
				.DisposeWith(SubscriptionDisposables);
		}
	}
}


