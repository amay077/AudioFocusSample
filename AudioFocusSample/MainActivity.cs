﻿using Android.App;
using Android.Widget;
using Android.OS;
using Android.Media;
using Android.Runtime;
using Android.Content;
using System.Threading.Tasks;

namespace AudioFocusSample
{
	[Activity(Label = "AudioFocusSample", MainLauncher = true, Icon = "@mipmap/icon")]
	public class MainActivity : Activity, AudioManager.IOnAudioFocusChangeListener
	{
		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.Main);

			var audioManager = (AudioManager)GetSystemService(Context.AudioService);

			// 先に効果音を読み込んでおく
			var soundPool = new SoundPool(1, Stream.Music, 0);
			var soundId = soundPool.Load(ApplicationContext, Resource.Raw.cat, 0);
			// SoundPool は再生完了のコールバックがないので、事前に長さを得ておく
			var duration = GetSoundDuration(Resource.Raw.cat);

			FindViewById<Button>(Resource.Id.buttonRequestFocus).Click += async (sender, e) =>
			{
			// ダッキングを許可する AudioFocus を要求
			var result = audioManager.RequestAudioFocus(this, Stream.Music, AudioFocus.GainTransientMayDuck);
				if (result == AudioFocusRequest.Granted)
				{
				// 効果音を再生する
				soundPool.Play(soundId, 1.0f, 1.0f, 0, 0, 1.0f);
				// 再生完了まで待つ
				await Task.Delay((int)duration);
				// AudioFocus を開放
				audioManager.AbandonAudioFocus(this);
				}
			};
		}

		// 音声の再生長さを得る
		private long GetSoundDuration(int rawId)
		{
			using (var player = MediaPlayer.Create(ApplicationContext, rawId))
			{
				return player.Duration;
			}
		}

		// IOnAudioFocusChangeListener の実装（RequestAudioFocus のために必要）
		public void OnAudioFocusChange([GeneratedEnum] AudioFocus focusChange)
		{
			// 今回は使用しない
		}
	}
}


