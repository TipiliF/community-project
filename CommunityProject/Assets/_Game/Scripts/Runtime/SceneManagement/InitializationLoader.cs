using BoundfoxStudios.CommunityProject.Events.ScriptableObjects;
using BoundfoxStudios.CommunityProject.SceneManagement.ScriptableObjects;
using BoundfoxStudios.CommunityProject.Settings.ScriptableObjects;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;

namespace BoundfoxStudios.CommunityProject.SceneManagement
{
	/// <summary>
	/// This component should be the very first component that is loaded by an initialization scene.
	/// It will handle bootstrapping the most necessary parts of the game to boot it.
	/// </summary>
	[AddComponentMenu(Constants.MenuNames.SceneManagement + "/" + nameof(InitializationLoader))]
	public class InitializationLoader : MonoBehaviour
	{
		[Header("Scenes")]
		[SerializeField]
		private AssetReferenceT<PersistentManagersSceneSO> PersistentManagersScene;

		[SerializeField]
		private AssetReferenceT<MenuSceneSO> MainMenuScene;

		[Header("Broadcasting Channels")]
		[SerializeField]
		private AssetReferenceT<LoadSceneEventChannelSO> LoadSceneEventChannel;

		[Header("GameSettings Reference")]
		[SerializeField]
		private AssetReferenceT<SettingsSO> Settings;

		[UsedImplicitly]
		// ReSharper disable once Unity.IncorrectMethodSignature
		private async UniTaskVoid Start()
		{
			await LoadSettingsAsync();
			await LoadPersistentManagersAsync();
			await LoadIntoMainMenuAsync();
			await UnloadInitializationSceneAsync();
		}

		private async UniTask LoadSettingsAsync()
		{
			await Settings.LoadAssetAsync();
		}

		private async UniTask LoadPersistentManagersAsync()
		{
			var persistentManagersScene = await PersistentManagersScene.LoadAssetAsync();
			await persistentManagersScene.SceneReference.LoadSceneAsync(LoadSceneMode.Additive);
		}

		private async UniTask LoadIntoMainMenuAsync()
		{
			var mainMenuScene = await MainMenuScene.LoadAssetAsync();
			var loadSceneEventChannel = await LoadSceneEventChannel.LoadAssetAsync();

			loadSceneEventChannel.Raise(new()
			{
				Scene = mainMenuScene,
				ShowLoadingScreen = true
			});
		}

		private async UniTask UnloadInitializationSceneAsync()
		{
			// Initialization Scene will always have build index 0.
			await SceneManager.UnloadSceneAsync(0);
		}
	}
}
