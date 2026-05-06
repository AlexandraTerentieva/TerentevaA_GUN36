using UnityEngine;

public class BoardGenerator : MonoBehaviour
{
    [SerializeField] private GameObject _cellPrefab;
    [SerializeField] private Material _darkMaterial;
    [SerializeField] private Material _lightMaterial;

    private void Start()
    {
        GenerateBoard();
    }

    private void GenerateBoard()
    {
        for (int x = 0; x < 8; x++)
        {
            for (int z = 0; z < 8; z++)
            {
                Vector3 pos = new Vector3(x, 0, z);
                GameObject cell = Instantiate(_cellPrefab, pos, Quaternion.identity, transform);
                cell.name = $"Cell_{x}_{z}";

                Renderer rend = cell.GetComponent<Renderer>();
                Material mat = (x + z) % 2 == 0 ? _lightMaterial : _darkMaterial;
                rend.material = mat;
            }
        }
        transform.position = new Vector3(-3.5f, 0, -3.5f);
    }
}