namespace Vasi
{
    public static class VasiGameObjectExtensions
    {
        public static GameObject? Child(this GameObject? gameObject, params string[] pathParts)
        {
            if (gameObject == null)
            {
                return null;
            }

            Transform? current = gameObject.transform;
            foreach (string part in pathParts)
            {
                if (current == null)
                {
                    return null;
                }

                current = current.Find(part);
            }

            return current?.gameObject;
        }

        public static GameObject? Child(this Component? component, params string[] pathParts) =>
            component == null ? null : component.gameObject.Child(pathParts);

        public static GameObject? Child(this Transform? transform, params string[] pathParts) =>
            transform == null ? null : transform.gameObject.Child(pathParts);

        public static PlayMakerFSM? LocateMyFSM(this GameObject? gameObject, string fsmName)
        {
            if (gameObject == null || string.IsNullOrEmpty(fsmName))
            {
                return null;
            }

            return gameObject
                .GetComponents<PlayMakerFSM>()
                .FirstOrDefault(fsm => fsm != null && string.Equals(fsm.FsmName, fsmName, StringComparison.Ordinal));
        }

        public static PlayMakerFSM? LocateMyFSM(this Component? component, string fsmName) =>
            component == null ? null : component.gameObject.LocateMyFSM(fsmName);

        public static void SetPosition2D(this Transform transform, float x, float y)
        {
            if (transform == null)
            {
                return;
            }

            Vector3 position = transform.position;
            position.x = x;
            position.y = y;
            transform.position = position;
        }

        public static void SetPosition2D(this GameObject gameObject, float x, float y) =>
            gameObject?.transform.SetPosition2D(x, y);

        public static void SetPositionX(this Transform transform, float x)
        {
            if (transform == null)
            {
                return;
            }

            Vector3 position = transform.position;
            position.x = x;
            transform.position = position;
        }

        public static void SetPositionY(this Transform transform, float y)
        {
            if (transform == null)
            {
                return;
            }

            Vector3 position = transform.position;
            position.y = y;
            transform.position = position;
        }

        public static TransformDelegate GetTransformDelegate(this GameObject gameObject) =>
            new(gameObject?.transform);
    }

    public sealed class TransformDelegate
    {
        private readonly Transform? transform;

        public TransformDelegate(Transform? transform)
        {
            this.transform = transform;
        }

        public float X
        {
            get => transform == null ? 0f : transform.position.x;
            set
            {
                if (transform == null)
                {
                    return;
                }

                Vector3 position = transform.position;
                position.x = value;
                transform.position = position;
            }
        }

        public float Y
        {
            get => transform == null ? 0f : transform.position.y;
            set
            {
                if (transform == null)
                {
                    return;
                }

                Vector3 position = transform.position;
                position.y = value;
                transform.position = position;
            }
        }

        public float Z
        {
            get => transform == null ? 0f : transform.position.z;
            set
            {
                if (transform == null)
                {
                    return;
                }

                Vector3 position = transform.position;
                position.z = value;
                transform.position = position;
            }
        }
    }
}
