using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Mirror;
using EZCameraShake;
using TMPro;

public class Player : NetworkBehaviour
{
	[Header("Movement")]
	public float speed;
	public float inAirSpeed;
	public float jumpHeight;
	public LayerMask groundLayer;
	[Header("GameAttributes")]
	public bool isTazed;
	public bool isPlunged;
	[SyncVar(hook = nameof(OnLivesChange))] public int Lives = 3;
	[Header("ParticlesSystem")]
	public ParticleSystem psjump;
	public ParticleSystem psWalk;
	public ParticleSystem Electirc;
	[Header("Other")]
	public Animator animator;
	public Transform ItemPos;
	public GameObject _camera;
	public CameraShaker cameraShaker;
	public HealthDisplay healthDisplay;
	public GameObject myModel;
	public LayerMask modelCastLayerMask;
	[Header("Customization")]
	[SyncVar] public string playerName = "";
	[SyncVar] public int playerColor = 0;
	[SyncVar] public int playerModel = 0;
	[SyncVar] public int playerEyeModel = 0;
	[SyncVar] public Vector3 playerEyeBrowParameters = Vector3.zero;
	[Header("Customization UI")]
	public TextMeshPro playerName_UI;
	public GameObject[] playerEyeBrows;
	public GameObject[] playerEyeBrowsMeshs;
	public SkinnedMeshRenderer playerColor_UI;
	public SkinnedMeshRenderer playerEye_UI;

	Rigidbody rb;
	bool canJump = true;
	public bool canMove = true;
	float DeathtimeLeft;

	private void Start()
	{
		rb = GetComponent<Rigidbody>();

		rb.freezeRotation = true;

		initialize();

		if (!isLocalPlayer)
			return;

		UnityEvent jump = new UnityEvent();
		jump.AddListener(delegate { Jump(); });
		Button button = GameControlsPanel.instace.jumpButton;

		button.onClick.RemoveAllListeners();
		button.onClick.AddListener(delegate { jump.Invoke(); });

		GameControlsPanel.instace.CreatingPlayerLoading.SetActive(false);
	}

	public void initialize()
	{
		playerName_UI.text = playerName;
		playerColor_UI.material.color = Customization.Instance.GetColor(playerColor);

		if (!Customization.InttoBool(Mathf.RoundToInt(playerEyeBrowParameters.z)))
		{
			foreach (var item in playerEyeBrowsMeshs)
			{
				item.SetActive(false);
			}
		}
		else
		{
			foreach (var item in playerEyeBrows)
			{
				item.transform.localScale = new Vector3(1, 1, playerEyeBrowParameters.x);
				item.GetComponent<CuveEyeBrowModifier>().curvness = playerEyeBrowParameters.y;
			}
		}

		playerColor_UI.sharedMesh = Customization.Instance.GetModel(playerModel);
		playerEye_UI.sharedMesh = Customization.Instance.GetEyeModel(playerEyeModel);
	}

	void OnLivesChange(int i , int _i)
    {
		healthDisplay.UpdateHearts(Lives);
	}

	GameObject otherPlunger = null;
	RaycastHit hit;
	private void Update()
	{
		if (GameControlsPanel.instace == null)
			return;

		//camera
		_camera.transform.parent = null;

		//ModelRoataion
		Physics.Raycast(transform.position, -Vector3.up, out hit, 100, modelCastLayerMask);
		myModel.transform.rotation = Quaternion.Lerp(myModel.transform.rotation, Quaternion.FromToRotation(transform.up, hit.normal), 5 * Time.deltaTime);

		//InGameAtrributes
		if (isPlunged)
        {
			if (otherPlunger == null)
			{
				canMove = true;
				isPlunged = false;
				rb.isKinematic = false;
				return;
			}

			canMove = false;
			transform.position = otherPlunger.transform.position + Vector3.up * 2;
			transform.rotation = otherPlunger.transform.rotation;
			rb.isKinematic = true;
		}

		animator.SetBool("isPlunged" , isPlunged);

		if (isTazed)
		{
			if (!Electirc.isPlaying)
			{
				Electirc.Play();
			}
		}
		else if (Electirc.isPlaying)
		{
			Electirc.Stop();
		}

		if (animator.GetBool("isWalk"))
		{
			if (!psWalk.isPlaying)
				psWalk.Play();
		}
		else
		{
			if (psWalk.isPlaying)
				psWalk.Stop();
		}

		animator.SetBool("isTazed", isTazed);

		if (!isLocalPlayer)
		{
			_camera.gameObject.SetActive(false);
					gameObject.layer = 12;
			return;
		}

		if (Input.GetKeyDown(KeyCode.T))
		{
			foreach (var item in GameControlsPanel.instace.UIelements)
			{
				item.gameObject.SetActive(!item.gameObject.activeInHierarchy);
			}
		}

		if (!GameControlsPanel.instace.UIelements[0].activeInHierarchy)
		{
			if (Input.GetKeyDown(KeyCode.E))
			{
				GameControlsPanel.instace.InteractButton.onClick.Invoke();
			}

			if (Input.GetKeyDown(KeyCode.Space))
			{
				GameControlsPanel.instace.jumpButton.onClick.Invoke();
			}
		}

		gameObject.layer = 6;

		////

		//Settuping Interact Button

		if (ItemPos.childCount > 0
			&& ItemPos.GetChild(0).GetComponent<Item>() != null
			&& ItemPos.GetChild(0).GetComponent<Item>().onInteract != null
			&& ItemPos.GetChild(0).GetComponent<Item>().canInteract )
		{
			GameControlsPanel.instace.SetInteractButton(ItemPos.GetChild(0).GetComponent<Item>().onInteract);
		}

		//RotationHandeling
		if (!canMove)
			return;

		var targetVelocity = new Vector3(GameControlsPanel.instace.touchJoystick.horizontalVirtualAxis, 0, GameControlsPanel.instace.touchJoystick.verticalVirtualAxis) + new Vector3(Input.GetAxisRaw("Horizontal") , 0 , Input.GetAxisRaw("Vertical")).normalized;
		Vector3 relativePos = (transform.position + targetVelocity) - transform.position;

		Quaternion rotation = Quaternion.LookRotation(relativePos, Vector3.up);

		if (targetVelocity != Vector3.zero)
		{
			myModel.transform.GetChild(0).localRotation = Quaternion.Lerp(myModel.transform.GetChild(0).localRotation, Quaternion.Euler(0, rotation.eulerAngles.y, 0), 10 * Time.deltaTime);
		}

		//Animations

		animator.SetBool("isWalk", targetVelocity != Vector3.zero);
		animator.SetBool("isJump", !OnGrounded());
	}

	public void ClientSideBoom(Vector3 pos , float explosionpower)
    {
		if (Vector3.Distance(transform.position , pos) <= 5)
			Stun(gameObject, pos, 1);

		rb.AddExplosionForce(explosionpower, pos, 5);
	}

	public void Jump()
	{
		if (OnGrounded())
		{
			canJump = true;
		}
	}

	float _speed = 0;

	private void FixedUpdate()
	{
		if (GameControlsPanel.instace == null)
			return;

		//CameraLerping
		_camera.transform.position = Vector3.Lerp(_camera.transform.position, myModel.transform.GetChild(0).GetChild(0).GetChild(0).transform.position, 2 * Time.deltaTime);


		if (!isLocalPlayer || !canMove)
			return;

		//Movement

		if (OnGrounded())
			_speed = Mathf.Lerp(_speed, speed, 10 * Time.deltaTime);
		else _speed = Mathf.Lerp(_speed, inAirSpeed, 5 * Time.deltaTime);

		Vector3 targetVelocity = new Vector3((GameControlsPanel.instace.touchJoystick.horizontalVirtualAxis + Input.GetAxisRaw("Horizontal")),
			0 , (GameControlsPanel.instace.touchJoystick.verticalVirtualAxis + Input.GetAxisRaw("Vertical"))).normalized;
		targetVelocity *= _speed;

		// Apply a force that attempts to reach our target velocity
		Vector3 velocity = rb.velocity * 5;
		Vector3 velocityChange = (targetVelocity - velocity);
		rb.AddForce(myModel.transform.right * velocityChange.x + myModel.transform.forward *  velocityChange.z);

		if (OnGrounded())
		{
			if (canJump)
			{
				rb.AddForce(myModel.transform.up * jumpHeight * Time.deltaTime, ForceMode.Impulse);
				canJump = false;
			}
		}
	}

	public bool OnGrounded()
	{
		CapsuleCollider col = GetComponent<CapsuleCollider>();

		return
		Physics.CheckCapsule(col.bounds.center, new Vector3(col.bounds.center.x,
			col.bounds.min.y, col.bounds.center.z), col.radius * .6f, groundLayer);
	}

	private void OnCollisionEnter(Collision collision)
	{
		psjump.transform.position = collision.GetContact(0).point;
		psjump.transform.rotation = Quaternion.FromToRotation(Vector3.up, collision.GetContact(0).normal);

		psjump.Play();
		cameraShaker.ShakeOnce(3, 1, 0.3f, 0.2f);

		if (!isServer)
			return;

		if (collision.collider.CompareTag("Player"))
		{
			if (ItemPos.childCount > 0)
			{
				if (ItemPos.GetChild(0).GetComponent<BombHandler>() != null)
				{
					ItemPos.GetChild(0).GetComponent<DestroyOverTime>().Die();
				}
				else if (ItemPos.GetChild(0).GetComponent<TazerHandler>() != null)
				{
					RpcTazed(collision.collider.GetComponent<NetworkIdentity>() , true);
					StartTaze(collision.collider.GetComponent<NetworkIdentity>());
					ItemPos.GetChild(0).GetComponent<DestroyOverTime>().Die();
				}
				else if (ItemPos.GetChild(0).GetComponent<PlungerHandler>() != null)
				{
					RpcPlunged(collision.collider.GetComponent<NetworkIdentity>().connectionToClient , ItemPos.GetChild(0).GetComponent<PlungerHandler>().gameObject);
				}
				else
				{
					DropItem(ItemPos.GetChild(0).gameObject);
				}
			}

			Stun(this.gameObject, collision.collider.GetComponent<Player>().gameObject.transform.position, 1);
		}

		if (collision.collider.CompareTag("Death"))
		{
			Die();
		}
	}

	[ClientRpc]
	public void RpcTazed(NetworkIdentity ntd , bool b)
    {
		ntd.GetComponent<Player>().isTazed = b;

		if (ntd == NetworkClient.connection.identity)
		{
			ntd.GetComponent<Player>().canMove = !b;
		}
	}

	public void StartTaze(NetworkIdentity ntd)
	{
		StartCoroutine(delay());

		IEnumerator delay()
		{
			yield return new WaitForSeconds(5);
			RpcTazed(ntd , false);
		}
	}

	[TargetRpc]
	public void RpcPlunged(NetworkConnection conn , GameObject thyPlunger)
	{
		NetworkClient.connection.identity.GetComponent<Player>().isPlunged = true;
		NetworkClient.connection.identity.GetComponent<Player>().otherPlunger = thyPlunger;
	}

	public void Stun(GameObject player , Vector3 otherPlayer, float time)
    {
		if (animator.GetCurrentAnimatorStateInfo(0).IsName("PlayerStunned") && enumisrun == false)
			return;

		RpcStunned(player , otherPlayer, time);
	}

	[HideInInspector]
	public bool enumisrun = false;

	[ClientRpc]
	public void RpcStunned(GameObject player, Vector3 position, float time)
	{
		Player p = player.GetComponent<Player>();

		StartCoroutine(delay());

		IEnumerator delay()
		{
			p.enumisrun = true;
			p.canMove = false;
			p.animator.SetBool("stunned", true);
			Vector3 relativePos = position - p.transform.position;
			p.myModel.transform.GetChild(0).localRotation = Quaternion.Euler(0, Quaternion.LookRotation(relativePos, Vector3.up).eulerAngles.y, 0);
			

			//KnockBack(20000 , -myModel.transform.GetChild(0).forward);

			yield return new WaitForSeconds(time);

			p.animator.SetBool("stunned", false);
			p.enumisrun = false;

			if (isTazed)
				yield break;
			p.canMove = true;
		}
	}

	public void KnockBack(float knockback , Vector3 direction)
    {
		rb.AddForce(direction * knockback , ForceMode.Impulse);
    }

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.cyan;
		Gizmos.DrawCube(hit.point, new Vector3(0.5f, 0.5f, 0.5f));

		CapsuleCollider col = GetComponent<CapsuleCollider>();

		if (OnGrounded())
			Gizmos.color = Color.red;
		else Gizmos.color = Color.green;
		Gizmos.DrawWireSphere(new Vector3(col.bounds.center.x,
			col.bounds.min.y, col.bounds.center.z), col.radius * .6f);
	}

	public void DropItem(GameObject item)
    {
		if (!isServer)
			CmdDropItem(item);
		else
        {
			item.GetComponent<Item>().isPicked = false;
			item.transform.SetParent(null);
			RcpDropItem(item);

			item.GetComponent<NetworkIdentity>().RemoveClientAuthority();
		}

	}

	[Command]
	public void CmdDropItem(GameObject item)
    {
		item.GetComponent<Item>().isPicked = false;
		item.transform.SetParent(null);
		RcpDropItem(item);

		item.GetComponent<NetworkIdentity>().RemoveClientAuthority();
	}

	[ClientRpc]
	private void RcpDropItem(GameObject item)
	{
		item.GetComponent<Item>().isPicked = false;
		item.transform.SetParent(null);

		if (NetworkClient.connection.identity.GetComponent<Player>().ItemPos.childCount == 0)
        {
			GameControlsPanel.instace.InteractButton.onClick.RemoveAllListeners();
        }
	}

	public void SetParent(GameObject item)
    {
		if (!item.GetComponent<Item>().canPick)
			return;

		if (ItemPos.childCount > 0)
        {
			DropItem(ItemPos.GetChild(0).gameObject);
		}

		item.GetComponent<Item>().isPicked = true;
		item.transform.SetParent(NetworkIdentity.spawned[netId].GetComponent<Player>().ItemPos.transform);

		if (item.GetComponent<NetworkIdentity>().hasAuthority)
			item.GetComponent<NetworkIdentity>().RemoveClientAuthority();

		item.GetComponent<NetworkIdentity>().AssignClientAuthority(GetComponent<NetworkIdentity>().connectionToClient);

		RcpSetParent(item , netId);
	}

	[ClientRpc]
	private void RcpSetParent(GameObject item , uint id)
	{
		item.GetComponent<Item>().isPicked = true;
		item.transform.SetParent(NetworkIdentity.spawned[id].GetComponent<Player>().ItemPos.transform);
		item.transform.localPosition = Vector3.zero;
		item.transform.localScale = new Vector3(3.333333f, 3.333333f, 3.333333f);
	}

	public void Die()
    {
		Lives -= 1;
		healthDisplay.UpdateHearts(Lives);

		Rpcdeath(this.netIdentity);

		if (Lives > 0)
			StartCoroutine(DeathDelay());
    }

	[ClientRpc]
	void Rpcdeath(NetworkIdentity player)
    {
		if (ItemPos.childCount > 0)
			DropItem(ItemPos.GetChild(0).gameObject);

		player.GetComponent<Player>().canMove = false;
		player.GetComponent<Player>().rb.isKinematic = true;
		player.transform.position = Vector3.zero + Vector3.up * 12;

		player.GetComponent<Player>().myModel.SetActive(false);
		player.GetComponent<CapsuleCollider>().enabled = false;
	}

	IEnumerator	DeathDelay()
    {
		RpcStartDeathCountDisplay(connectionToClient);
		yield return new WaitForSeconds(5);
		Revivie();
	}

	[TargetRpc]
	public void RpcStartDeathCountDisplay(NetworkConnection conn)
    {
		StartCoroutine(DeathDelay());

		IEnumerator DeathDelay()
		{
			for (DeathtimeLeft = 5; DeathtimeLeft > 0; DeathtimeLeft -= Time.deltaTime)
			{
				GameControlsPanel.instace.gameInteractionDisplay.text = Mathf.Round(DeathtimeLeft).ToString();
				yield return null;
			}

			GameControlsPanel.instace.gameInteractionDisplay.text = "";
		}
	}

	public void Revivie()
	{
		RpcRevivie(this.netIdentity);
	}

	[ClientRpc]
	void RpcRevivie(NetworkIdentity player)
	{
		player.GetComponent<Player>().canMove = true;
		player.GetComponent<Player>().rb.isKinematic = false;
		player.transform.position = Vector3.zero + Vector3.up * 8;

		player.GetComponent<Player>().myModel.SetActive(true);
		player.GetComponent<Player>().GetComponent<CapsuleCollider>().enabled = true;
	}
}
