using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Music
{
   public class JingleManager : MonoBehaviour
   {
      [FormerlySerializedAs("jingleSource")] [SerializeField] private AudioSource _jingleSource;
      [Space]
      [FormerlySerializedAs("win")] [SerializeField] private AudioClip _win;
      [FormerlySerializedAs("lose")] [SerializeField] private AudioClip _lose;

      public static Action PlayWinJingle;
      public static Action PlayLoseJingle;

      private void OnEnable()
      {
         PlayWinJingle += PlayWinJingleClip;
         PlayLoseJingle += PlayLoseJingleClip;
      }
   
      private void OnDisable()
      {
         PlayWinJingle -= PlayWinJingleClip;
         PlayLoseJingle -= PlayLoseJingleClip;
      }
   
      private void PlayWinJingleClip() => 
         _jingleSource.PlayOneShot(_win);

      private void PlayLoseJingleClip() => 
         _jingleSource.PlayOneShot(_lose);
   }
}
