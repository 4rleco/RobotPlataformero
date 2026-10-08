using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerEvents : MonoBehaviour
{
    public static PlayerEvents current;

    public event Action<PlayerActions> onJumpTutorialTriggerEnter;

    public event Action<PlayerActions> onDashTutorialTriggerEnter;

    public event Action<PlayerActions> onSlideTutorialTriggerEnter;

    public event Action<PlayerActions> onPlayerJumpTriggerClose;

    public event Action<PlayerActions> onPlayerDashTriggerClose;

    public event Action<PlayerActions> onPlayerSlideTriggerClose;

    public event Action<PlayerActions> onPlayerDeath;

    private void Awake()
    {
        current = this;
    }

    public void OnJumpTutorialTriggerEnter(PlayerActions player)
    {
        if (onJumpTutorialTriggerEnter != null)
        {
            onJumpTutorialTriggerEnter.Invoke(player);
        }
    }

    public void OnDashTutorialTriggerEnter(PlayerActions player)
    {
        if (onDashTutorialTriggerEnter != null)
        {
            onDashTutorialTriggerEnter.Invoke(player);
        }
    }

    public void OnSlideTutorialTriggerEnter(PlayerActions player)
    {
        if (onSlideTutorialTriggerEnter != null)
        {
            onSlideTutorialTriggerEnter.Invoke(player);
        }
    }

    public void OnPlayerJumpTriggerClose(PlayerActions player)
    {
        if (onPlayerJumpTriggerClose != null)
        {
            onPlayerJumpTriggerClose.Invoke(player);
        }
    }
    public void OnPlayerDashTriggerClose(PlayerActions player)
    {
        if (onPlayerDashTriggerClose != null)
        {
            onPlayerDashTriggerClose.Invoke(player);
        }
    }
    public void OnPlayerSlideTriggerClose(PlayerActions player)
    {
        if (onPlayerSlideTriggerClose != null)
        {
            onPlayerSlideTriggerClose.Invoke(player);
        }
    }

    public void OnPlayerDeath(PlayerActions player)
    {
        if (onPlayerDeath != null)
        {
            onPlayerDeath.Invoke(player);
        }
    }
}