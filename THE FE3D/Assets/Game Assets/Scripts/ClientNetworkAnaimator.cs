using UnityEngine;
using Unity.Netcode.Components;

[RequireComponent(typeof(Animator))]
public class ClientNetworkAnimator : NetworkAnimator
{
    protected override bool OnIsServerAuthoritative()
    {
        return false; // Client controls animation
    }
}
