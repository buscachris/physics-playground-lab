using UnityEngine;
using UnityEngine.UI;

namespace PhysicsPlaygroundLab
{
    public sealed class PhysicsPlaygroundApp : MonoBehaviour
    {
        private Material groundMaterial;
        private Material propMaterial;
        private Material accentMaterial;
        private Material forceMaterial;
        private Material jumpMaterial;
        private Material playerMaterial;
        private PhysicsPlayer player;
        private PhysicsCamera physicsCamera;
        private PhysicsTelekinesis telekinesis;
        private Text statusText;
        private Text modeText;

        private void Start()
        {
            Application.targetFrameRate = 60;
            BuildMaterials();
            BuildWorld();
            BuildPlayerAndCamera();
            BuildUi();
            SetStatus("Physics playground online. Right-click to look; left/right click push or pull rigidbodies.");
        }

        private void BuildMaterials()
        {
            groundMaterial = Material(new Color(0.07f, 0.10f, 0.15f));
            propMaterial = Material(new Color(0.25f, 0.45f, 0.75f));
            accentMaterial = Material(new Color(0.90f, 0.33f, 0.20f));
            forceMaterial = Material(new Color(0.55f, 0.18f, 1f, 0.35f));
            jumpMaterial = Material(new Color(0.15f, 1f, 0.65f));
            playerMaterial = Material(new Color(1f, 0.78f, 0.20f));
        }

        private Material Material(Color color)
        {
            Material material = new Material(Shader.Find("Standard")) { color = color };
            material.SetFloat("_Metallic", 0.18f);
            material.SetFloat("_Glossiness", 0.35f);
            if (color.a < 1f)
            {
                material.SetFloat("_Mode", 3f);
                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                material.SetInt("_ZWrite", 0);
                material.EnableKeyword("_ALPHABLEND_ON");
                material.renderQueue =  transparentQueue;
            }
            return material;
        }

        private int transparentQueue => 3000;

        private void BuildWorld()
        {
            Cube("Physics Floor", new Vector3(0f, -0.3f, 8f), new Vector3(34f, 0.5f, 38f), groundMaterial);
            Cube("North Wall", new Vector3(0f, 3f, 26.5f), new Vector3(34f, 6f, 0.5f), propMaterial);
            Cube("West Wall", new Vector3(-16.5f, 3f, 8f), new Vector3(0.5f, 6f, 38f), propMaterial);
            Cube("East Wall", new Vector3(16.5f, 3f, 8f), new Vector3(0.5f, 6f, 38f), propMaterial);

            Rigidbody crate = RigidbodyCube("Telekinesis Crate", new Vector3(3f, 1.25f, 5f), new Vector3(2.2f, 2.2f, 2.2f), accentMaterial, 3f);
            crate.angularDrag = 0.35f;
            Rigidbody barrel = RigidbodyCylinder("Rolling Barrel", new Vector3(-3f, 1.2f, 7f), new Vector3(1.5f, 1.2f, 1.5f), propMaterial, 2f);
            barrel.angularDrag = 0.2f;

            CreateForceField(new Vector3(-7f, 2f, 5f), new Vector3(4f, 4f, 5f), new Vector3(0f, 0f, 10f), "WIND FIELD");
            CreateJumpPad(new Vector3(7f, 0.15f, 5f), new Vector3(3f, 0.25f, 3f), "JUMP PAD");
            CreateMovingPlatform();
            CreateSwing();

            Cube("Wall Jump Surface", new Vector3(0f, 2f, 17f), new Vector3(7f, 4f, 0.5f), accentMaterial);
            Cube("Physics Ramp", new Vector3(-8f, 1f, 14f), new Vector3(5f, 0.5f, 10f), propMaterial).transform.rotation = Quaternion.Euler(-12f, 0f, 0f);
        }

        private void BuildPlayerAndCamera()
        {
            GameObject playerObject = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            playerObject.name = "Physics Player";
            playerObject.transform.position = new Vector3(0f, 1.2f, 0f);
            playerObject.GetComponent<Renderer>().sharedMaterial = playerMaterial;
            Destroy(playerObject.GetComponent<CapsuleCollider>());
            CharacterController controller = playerObject.AddComponent<CharacterController>();
            controller.height = 1.8f;
            controller.radius = 0.38f;
            controller.center = new Vector3(0f, 0.9f, 0f);
            player = playerObject.AddComponent<PhysicsPlayer>();

            GameObject cameraObject = new GameObject("Physics Camera");
            Camera camera = cameraObject.AddComponent<Camera>();
            camera.fieldOfView = 72f;
            physicsCamera = cameraObject.AddComponent<PhysicsCamera>();
            physicsCamera.Target = playerObject.transform;
            physicsCamera.View = camera;
            player.View = cameraObject.transform;

            telekinesis = playerObject.AddComponent<PhysicsTelekinesis>();
            telekinesis.View = camera;
            telekinesis.Player = player;
        }

        private void BuildUi()
        {
            GameObject canvasObject = new GameObject("Physics Playground UI");
            Canvas canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            CanvasScaler scaler = canvasObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1280f, 720f);
            canvasObject.AddComponent<GraphicRaycaster>();

            Text title = Label(canvasObject.transform, "Title", new Vector2(0f, -18f), new Vector2(1f, 44f), 24, TextAnchor.MiddleLeft);
            title.text = "PHYSICS PLAYGROUND LAB";
            modeText = Label(canvasObject.transform, "Mode", new Vector2(0f, -55f), new Vector2(1f, 32f), 17, TextAnchor.MiddleLeft);
            statusText = Label(canvasObject.transform, "Status", new Vector2(0f, 30f), new Vector2(1f, 42f), 17, TextAnchor.MiddleCenter);
            Text help = Label(canvasObject.transform, "Help", new Vector2(0f, 68f), new Vector2(1f, 48f), 15, TextAnchor.MiddleCenter);
            help.text = "WASD move | SPACE jump / wall jump | V first/third person | scroll zoom | ESC unlocks cursor";
            Text crosshair = Label(canvasObject.transform, "Crosshair", Vector2.zero, new Vector2(60f, 60f), 30, TextAnchor.MiddleCenter);
            crosshair.text = "+";
            telekinesis.Status = statusText;
        }

        private Text Label(Transform parent, string name, Vector2 position, Vector2 size, int fontSize, TextAnchor alignment)
        {
            GameObject root = new GameObject(name);
            root.transform.SetParent(parent, false);
            RectTransform rect = root.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0f, 1f);
            rect.anchorMax = new Vector2(1f, 1f);
            rect.pivot = new Vector2(0.5f, 1f);
            rect.anchoredPosition = position;
            rect.sizeDelta = size;
            Text label = root.AddComponent<Text>();
            label.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            label.fontSize = fontSize;
            label.color = Color.white;
            label.alignment = alignment;
            return label;
        }

        private GameObject Cube(string name, Vector3 position, Vector3 scale, Material material)
        {
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.name = name;
            cube.transform.position = position;
            cube.transform.localScale = scale;
            cube.GetComponent<Renderer>().sharedMaterial = material;
            return cube;
        }

        private Rigidbody RigidbodyCube(string name, Vector3 position, Vector3 scale, Material material, float mass)
        {
            GameObject cube = Cube(name, position, scale, material);
            Rigidbody rigidbody = cube.AddComponent<Rigidbody>();
            rigidbody.mass = mass;
            return rigidbody;
        }

        private Rigidbody RigidbodyCylinder(string name, Vector3 position, Vector3 scale, Material material, float mass)
        {
            GameObject cylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            cylinder.name = name;
            cylinder.transform.position = position;
            cylinder.transform.localScale = scale;
            cylinder.GetComponent<Renderer>().sharedMaterial = material;
            Rigidbody rigidbody = cylinder.AddComponent<Rigidbody>();
            rigidbody.mass = mass;
            return rigidbody;
        }

        private void CreateForceField(Vector3 position, Vector3 scale, Vector3 force, string label)
        {
            GameObject field = Cube(label, position, scale, forceMaterial);
            BoxCollider trigger = field.GetComponent<BoxCollider>();
            trigger.isTrigger = true;
            ForceField forceField = field.AddComponent<ForceField>();
            forceField.Force = force;
        }

        private void CreateJumpPad(Vector3 position, Vector3 scale, string label)
        {
            GameObject pad = Cube(label, position, scale, jumpMaterial);
            BoxCollider trigger = pad.GetComponent<BoxCollider>();
            trigger.isTrigger = true;
            JumpPad jumpPad = pad.AddComponent<JumpPad>();
            jumpPad.Strength = 12f;
        }

        private void CreateMovingPlatform()
        {
            GameObject platform = Cube("Moving Platform", new Vector3(-5f, 4f, 10f), new Vector3(5f, 0.5f, 5f), propMaterial);
            MovingPlatform mover = platform.AddComponent<MovingPlatform>();
            mover.PointA = new Vector3(-5f, 4f, 10f);
            mover.PointB = new Vector3(5f, 4f, 10f);
            mover.Speed = 1.4f;
        }

        private void CreateSwing()
        {
            GameObject anchor = new GameObject("Swing Anchor");
            anchor.transform.position = new Vector3(8f, 10f, 15f);
            Rigidbody anchorBody = anchor.AddComponent<Rigidbody>();
            anchorBody.isKinematic = true;
            Rigidbody previous = anchorBody;
            for (int i = 0; i < 6; i++)
            {
                Rigidbody link = RigidbodyCube("Swing Link " + i, new Vector3(8f, 9f - i * 1.25f, 15f), new Vector3(0.8f, 1.1f, 0.8f), accentMaterial, 0.8f);
                ConfigurableJoint joint = link.gameObject.AddComponent<ConfigurableJoint>();
                joint.connectedBody = previous;
                joint.xMotion = ConfigurableJointMotion.Limited;
                joint.yMotion = ConfigurableJointMotion.Limited;
                joint.zMotion = ConfigurableJointMotion.Limited;
                joint.linearLimit = new SoftJointLimit { limit = 0.65f };
                previous = link;
            }
            Rigidbody seat = RigidbodyCube("Swing Seat", new Vector3(8f, 1.5f, 15f), new Vector3(3.5f, 0.35f, 2f), jumpMaterial, 2f);
            ConfigurableJoint seatJoint = seat.gameObject.AddComponent<ConfigurableJoint>();
            seatJoint.connectedBody = previous;
            seatJoint.xMotion = ConfigurableJointMotion.Limited;
            seatJoint.yMotion = ConfigurableJointMotion.Limited;
            seatJoint.zMotion = ConfigurableJointMotion.Limited;
            seatJoint.linearLimit = new SoftJointLimit { limit = 0.65f };
        }

        private void SetStatus(string message)
        {
            if (statusText != null) statusText.text = message;
            if (modeText != null) modeText.text = "Camera: " + (physicsCamera != null && physicsCamera.FirstPerson ? "FIRST PERSON" : "THIRD PERSON") + "  |  Telekinesis: left pull / right push";
        }

        public void UpdateCameraMode(bool firstPerson)
        {
            if (modeText != null) modeText.text = "Camera: " + (firstPerson ? "FIRST PERSON" : "THIRD PERSON") + "  |  Telekinesis: left pull / right push";
        }
    }

    public sealed class PhysicsPlayer : MonoBehaviour
    {
        public Transform View;
        private CharacterController controller;
        private Vector3 velocity;
        private Vector3 externalVelocity;
        private float gravity = -22f;

        private void Awake()
        {
            controller = GetComponent<CharacterController>();
        }

        private void Update()
        {
            Vector3 forward = View == null ? Vector3.forward : View.forward;
            Vector3 right = View == null ? Vector3.right : View.right;
            forward.y = 0f;
            right.y = 0f;
            forward.Normalize();
            right.Normalize();
            Vector3 input = forward * Input.GetAxisRaw("Vertical") + right * Input.GetAxisRaw("Horizontal");
            if (input.sqrMagnitude > 1f) input.Normalize();
            velocity.x = Mathf.Lerp(velocity.x, input.x * 6f, 12f * Time.deltaTime);
            velocity.z = Mathf.Lerp(velocity.z, input.z * 6f, 12f * Time.deltaTime);

            if (controller.isGrounded)
            {
                if (velocity.y < 0f) velocity.y = -2f;
                if (Input.GetKeyDown(KeyCode.Space)) velocity.y = 9f;
            }
            else if (Input.GetKeyDown(KeyCode.Space) && Physics.Raycast(transform.position, WallDirection(input), out RaycastHit wall, 1.1f))
            {
                velocity = wall.normal * 8f + Vector3.up * 9f;
            }

            velocity.y += gravity * Time.deltaTime;
            Vector3 motion = (velocity + externalVelocity) * Time.deltaTime;
            controller.Move(motion);
            externalVelocity = Vector3.Lerp(externalVelocity, Vector3.zero, 4f * Time.deltaTime);
        }

        private Vector3 WallDirection(Vector3 input)
        {
            if (input.sqrMagnitude > 0.01f) return input.normalized;
            return transform.forward;
        }

        public void AddExternalVelocity(Vector3 amount)
        {
            externalVelocity += amount;
        }

        public void AddPlatformDelta(Vector3 delta)
        {
            if (controller != null && controller.isGrounded) controller.Move(delta);
        }
    }

    public sealed class PhysicsCamera : MonoBehaviour
    {
        public Transform Target { get; set; }
        public Camera View { get; set; }
        public bool FirstPerson { get; private set; } = true;

        private float yaw;
        private float pitch;
        private float distance = 6f;

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void LateUpdate()
        {
            if (Target == null || View == null) return;
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            if (Input.GetMouseButtonDown(1))
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }

            yaw += Input.GetAxis("Mouse X") * 3.5f;
            pitch = Mathf.Clamp(pitch - Input.GetAxis("Mouse Y") * 3.5f, -75f, 75f);
            if (Input.GetKeyDown(KeyCode.V)) FirstPerson = !FirstPerson;
            distance = Mathf.Clamp(distance - Input.GetAxis("Mouse ScrollWheel") * 3f, 2.5f, 12f);

            Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);
            if (FirstPerson)
            {
                transform.position = Target.position + Vector3.up * 1.55f;
                transform.rotation = rotation;
            }
            else
            {
                transform.position = Target.position + Vector3.up * 1.5f - rotation * Vector3.forward * distance;
                transform.rotation = rotation;
            }
        }
    }

    public sealed class PhysicsTelekinesis : MonoBehaviour
    {
        public Camera View;
        public PhysicsPlayer Player;
        public Text Status;
        private Rigidbody target;

        private void Update()
        {
            if (View == null) return;
            Ray ray = View.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
            target = null;
            if (Physics.Raycast(ray, out RaycastHit hit, 12f)) target = hit.rigidbody;
            bool canUse = target != null && target.gameObject.layer != LayerMask.NameToLayer("Unmovable");
            if (!canUse)
            {
                if (Status != null) Status.text = "Telekinesis idle — aim at a Rigidbody within range.";
                return;
            }

            float distance = Vector3.Distance(Player.transform.position, target.position);
            if (distance > 12f)
            {
                if (Status != null) Status.text = "Target is out of telekinesis range.";
                return;
            }

            if (Input.GetMouseButton(0))
            {
                Vector3 pull = (Player.transform.position + Vector3.up * 1.2f - target.position).normalized;
                target.AddForce(pull * 32f, ForceMode.Acceleration);
                if (Status != null) Status.text = "PULLING " + target.name;
            }
            else if (Input.GetMouseButton(1))
            {
                Vector3 push = (target.position - Player.transform.position).normalized;
                target.AddForce(push * 42f, ForceMode.Acceleration);
                if (Status != null) Status.text = "PUSHING " + target.name;
            }
            else if (Status != null)
            {
                Status.text = "Valid target: " + target.name + " — left pull / right push.";
            }
        }
    }

    public sealed class MovingPlatform : MonoBehaviour
    {
        public Vector3 PointA;
        public Vector3 PointB;
        public float Speed = 1f;
        private Vector3 previousPosition;

        private void Start()
        {
            previousPosition = transform.position;
        }

        private void Update()
        {
            transform.position = Vector3.Lerp(PointA, PointB, (Mathf.Sin(Time.time * Speed) + 1f) * 0.5f);
            Vector3 delta = transform.position - previousPosition;
            PhysicsPlayer player = FindObjectOfType<PhysicsPlayer>();
            if (player != null && Vector3.Distance(player.transform.position, transform.position) < 3.2f) player.AddPlatformDelta(delta);
            previousPosition = transform.position;
        }
    }

    public sealed class ForceField : MonoBehaviour
    {
        public Vector3 Force = Vector3.forward;

        private void OnTriggerStay(Collider other)
        {
            if (other.attachedRigidbody != null) other.attachedRigidbody.AddForce(Force, ForceMode.Acceleration);
            PhysicsPlayer player = other.GetComponent<PhysicsPlayer>();
            if (player != null) player.AddExternalVelocity(Force * Time.fixedDeltaTime);
        }
    }

    public sealed class JumpPad : MonoBehaviour
    {
        public float Strength = 12f;

        private void OnTriggerEnter(Collider other)
        {
            if (other.attachedRigidbody != null) other.attachedRigidbody.AddForce(Vector3.up * Strength, ForceMode.Impulse);
            PhysicsPlayer player = other.GetComponent<PhysicsPlayer>();
            if (player != null) player.AddExternalVelocity(Vector3.up * Strength * 0.8f);
        }
    }
}
