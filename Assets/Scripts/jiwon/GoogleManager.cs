// using Firebase.Auth;
// using UnityEngine;
// using UnityEngine.UI;
// using GooglePlayGames;
// using GooglePlayGames.BasicApi;
// using System.Collections;
// using System.Collections.Generic;
// using TMPro;


// public class GoogleManager : MonoBehaviour
// {
//     public TMP_Text googleLog;
//     public TMP_Text firebaseLog;

//     FirebaseAuth fbauth;

//     // Start is called before the first frame update
//     void Start()
//     {
//         PlayGamesPlatform.InitializeInstance(new PlayGamesClientConfiguration.Builder()
//             .RequestIdToken()
//             .RequestEmail()
//             .Build());
//         PlayGamesPlatform.DebugLogEnabled = true;
//         PlayGamesPlatform.Activate();

//         fbauth = FirebaseAuth.DefaultInstance;

//         TryGoogleLogin();
//     }

//     public void TryGoogleLogin()
//     {
//         PlayGamesPlatform.Instance.Authenticate(SignInInteractivity.CanPromptAlways, (success) =>
//         {
//             if (success == SignInStatus.Success)
//             {
//                 googleLog.text = "Google Success";
//                 StartCoroutine(TryFirebaseLogin());
//             }
//             else
//             {
//                 googleLog.text = "google Failure";
//             }
//         });
//     }
//     public void TryGoggleLogout()
//     {
//         if (Social.localUser.authenticated)
//         {
//             PlayGamesPlatform.Instance.SignOut();
//             fbauth.SignOut();
//         }
//     }
//     IEnumerator TryFirebaseLogin()
//     {
//         while (string.IsNullOrEmpty(((PlayGamesLocalUser)Social.localUser).GetIdToken()))
//         {
//             yield return null;
//         }

//         string idToken = ((PlayGamesLocalUser)Social.localUser).GetIdToken();

//         Credential credential = GoogleAuthProvider.GetCredential(idToken, null);

//         fbauth.SignInWithCredentialAsync(credential).ContinueWith(task =>
//         {
//             if (task.IsCanceled)
//                 firebaseLog.text = "Firebase Cancles";
//             else if (task.IsFaulted)
//                 firebaseLog.text = "firebase Faulted";
//             else
//                 firebaseLog.text = "firebase success";
//         });
//     }
// }