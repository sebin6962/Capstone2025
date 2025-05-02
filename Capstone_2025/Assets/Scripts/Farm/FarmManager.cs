using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
public class FarmManager : MonoBehaviour
{
    public Tilemap fieldTilemap; // 밭이 칠해진 Tilemap
    public Tilemap overlayTilemap; //상태 변화 시 겹쳐질 Tilemap
    public TileBase farmTile;  // 밭으로 간주할 타일 (FarmSoilTile.asset) // 마른 흙 타일
    public TileBase wetSoilTile; // 젖은 흙 타일
    public Tilemap seedOverlayTilemap;   // 씨앗 타일 전용
    public TileBase seedTile;           // 씨앗 스프라이트 타일 (ex: seedTile.asset)

    private HashSet<Vector3Int> farmPositions = new HashSet<Vector3Int>();

    void Start()
    {
        RegisterFarmTiles();
    }

    // 1. 타일맵에서 밭 범위 자동 등록
    void RegisterFarmTiles()
    {
        farmPositions.Clear();

        // 범위를 스캔 (유효한 영역 내에서만)
        BoundsInt bounds = fieldTilemap.cellBounds;

        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                Vector3Int pos = new Vector3Int(x, y, 0);
                TileBase tile = fieldTilemap.GetTile(pos);

                if (tile == farmTile)
                {
                    farmPositions.Add(pos);
                }
            }
        }

        Debug.Log($"밭 위치 {farmPositions.Count}개 등록 완료");
    }

    // 2. 이 위치가 밭인가?
    public bool IsFarmTile(Vector3 worldPos)
    {
        Vector3Int cellPos = fieldTilemap.WorldToCell(worldPos);
        return farmPositions.Contains(cellPos);
    }

    // 3. 추후 밭 범위 확장 (예: 레벨업)
    public void AddFarmTile(Vector3Int cellPos)
    {
        farmPositions.Add(cellPos);
        fieldTilemap.SetTile(cellPos, farmTile);
    }

    //밭에 물 뿌렸을 때 변화
    public void WaterSoil(Vector3 worldPos)
    {
        Vector3Int cellPos = fieldTilemap.WorldToCell(worldPos);

        if (IsFarmTile(worldPos))
        {
            overlayTilemap.SetTile(cellPos, wetSoilTile);
        }
    }

    public void PlantSeed(Vector3 worldPos)
    {
        Vector3Int cellPos = fieldTilemap.WorldToCell(worldPos);

        if (IsFarmTile(worldPos))
        {
            seedOverlayTilemap.SetTile(cellPos, seedTile);
        }
    }
}
