using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MissileSimulation.Replay
{
	public class ReplayManager : Singleton<ReplayManager>
	{
		[SerializeField] private Camera _mainCamera;

		[SerializeField] private float _delayBetweenFrames = 0.05f;
		[SerializeField] private List<Texture2D> _frames = new List<Texture2D> ();
		[SerializeField] private Renderer _quadRenderer;
		[SerializeField] private RawImage _theUI;

		private bool _isRecording = false;
		private bool _isPlaying = false;

        public void StartRecording()
        {
           _frames.Clear();
			_isRecording = true;
        }

		public void StopRecording()
		{
			_isRecording = false;
		}

		public void RecordFrame()
		{
            if (_isRecording)
            {
                Texture2D frame = CaptureFrame();
                _frames.Add(frame);
            }
        }

		private Texture2D CaptureFrame()
		{
			RenderTexture currentRT = RenderTexture.active;
			RenderTexture.active = _mainCamera.targetTexture;

            Texture2D frame = new Texture2D(
							 _mainCamera.targetTexture.width,
							 _mainCamera.targetTexture.height,
							 TextureFormat.RGB24, // or ARGB32 if you need alpha
							 false, // mipmap
							 true   // linear
						 );
            frame.ReadPixels(new Rect(0, 0, _mainCamera.targetTexture.width, _mainCamera.targetTexture.height), 0, 0);
			frame.Apply();

			RenderTexture.active = currentRT;

			return frame;
		}

		private void DisplayFrame(Texture2D frame)
		{
            _theUI.texture = frame;
		}

		public void StartPlayback()
		{
			if(!_isPlaying && _frames.Count > 0)
			{
				StartCoroutine(Playback());
			}
        }

		IEnumerator Playback()
		{
			_isPlaying = true;
			_theUI.gameObject.SetActive(true);

            for (int i = 0; i < _frames.Count; i++)
			{
				DisplayFrame(_frames[i]);
				yield return new WaitForSeconds(_delayBetweenFrames);
			}
			_isPlaying = false;
            yield return new WaitForSeconds(1);
            GameManager.GetInstance().SetGameOver(true);
		}

        private void LateUpdate()
        {
            RecordFrame();
        }
    } 
}
