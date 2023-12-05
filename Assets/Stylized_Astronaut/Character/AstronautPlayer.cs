using UnityEngine;
using System.Collections;
using Photon.Pun;

namespace AstronautPlayer
{
    public class AstronautPlayer : MonoBehaviourPun
    {
        private Animator anim;
        private CharacterController controller;

        public float speed = 600.0f;
        public float turnSpeed = 400.0f;
        private Vector3 moveDirection = Vector3.zero;
        public float gravity = 20.0f;

        void Start()
        {
            controller = GetComponent<CharacterController>();
            anim = gameObject.GetComponentInChildren<Animator>();

            // Disable animations for remote players
            if (!photonView.IsMine)
            {
                anim.enabled = false;
            }
        }

        void Update()
        {
            if (photonView.IsMine)
            {
                if (Input.GetKey("w"))
                {
                    anim.SetInteger("AnimationPar", 1);
                }
                else
                {
                    anim.SetInteger("AnimationPar", 0);
                }

                if (controller.isGrounded)
                {
                    moveDirection = transform.forward * Input.GetAxis("Vertical") * speed;
                }

                float turn = Input.GetAxis("Horizontal");
                transform.Rotate(0, turn * turnSpeed * Time.deltaTime, 0);
                controller.Move(moveDirection * Time.deltaTime);
                moveDirection.y -= gravity * Time.deltaTime;

                // Send animation state to other players
                photonView.RPC("SyncAnimationState", RpcTarget.Others, anim.GetInteger("AnimationPar"));
            }
        }

        [PunRPC]
        void SyncAnimationState(int animationState)
        {
            // Receive animation state from the network and apply it to the remote player's animator
            anim.SetInteger("AnimationPar", animationState);
        }
    }
}
