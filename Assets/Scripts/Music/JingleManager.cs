using System;
using UnityEngine;

public class JingleManager : MonoBehaviour
{
   [SerializeField] private AudioSource jingleSource;
   [Space]
   [SerializeField] private AudioClip win;
   [SerializeField] private AudioClip lose;

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
   
   private void PlayWinJingleClip()
   {
      jingleSource.PlayOneShot(win);
   }
   
   private void PlayLoseJingleClip()
   {
      jingleSource.PlayOneShot(lose);
   }
}
