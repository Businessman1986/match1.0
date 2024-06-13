using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Tile : MonoBehaviour
{
    private static Color selectedColor = new Color(.5f, .5f, .5f, 1.0f);
    private static Tile previousSelected = null;

    private SpriteRenderer render;
    private bool isSelected = false;

    private Vector2[] adjacentDirections = new Vector2[] { Vector2.up, Vector2.down, Vector2.left, Vector2.right };
    private bool matchFound = false;

    void Awake()
    {
        render = GetComponent<SpriteRenderer>();
    }

    private void Select()
    {
        isSelected = true;
        render.color = selectedColor;
        previousSelected = gameObject.GetComponent<Tile>();
        SFXManager.instance.PlaySFX(Clip.Select);
    }

    private void Deselect()
    {
        isSelected = false;
        render.color = Color.white;
        previousSelected = null;
    }

    void OnMouseDown()
    {
        if (render.sprite == null || BoardManager.instance.IsShifting)
        {
            return;
        }

        if (isSelected)
        { // Is it already selected?
            Deselect();
        }
        else
        {
            if (previousSelected == null)
            { // Is it the first tile selected?
                Select();
            }
            else
            {
                if (GetAllAdjacentTiles().Contains(previousSelected.gameObject))
                {
                    StartCoroutine(TryMatch(previousSelected));
                }
                else
                {
                    previousSelected.Deselect();
                    Select();
                }
            }
        }
    }

    private IEnumerator TryMatch(Tile otherTile)
    {
        SwapSprite(otherTile.render);
        yield return new WaitForSeconds(0.5f); // Small delay to show the swap

        if (!HasMatch() && !otherTile.HasMatch())
        {
            // If no matches, swap back
            SwapSprite(otherTile.render);
        }

        otherTile.ClearAllMatches();
        otherTile.Deselect();
        ClearAllMatches();
    }

    public void SwapSprite(SpriteRenderer render2)
    {
        if (render.sprite == render2.sprite)
        {
            return;
        }

        Sprite tempSprite = render2.sprite;
        render2.sprite = render.sprite;
        render.sprite = tempSprite;
        SFXManager.instance.PlaySFX(Clip.Swap);
    }

    private GameObject GetAdjacent(Vector2 castDir)
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, castDir);
        if (hit.collider != null)
        {
            return hit.collider.gameObject;
        }
        return null;
    }

    private List<GameObject> GetAllAdjacentTiles()
    {
        List<GameObject> adjacentTiles = new List<GameObject>();
        for (int i = 0; i < adjacentDirections.Length; i++)
        {
            adjacentTiles.Add(GetAdjacent(adjacentDirections[i]));
        }
        return adjacentTiles;
    }

    private List<GameObject> FindMatch(Vector2 castDir)
    {
        List<GameObject> matchingTiles = new List<GameObject>();
        RaycastHit2D hit = Physics2D.Raycast(transform.position, castDir);
        while (hit.collider != null && hit.collider.GetComponent<SpriteRenderer>().sprite == render.sprite)
        {
            matchingTiles.Add(hit.collider.gameObject);
            hit = Physics2D.Raycast(hit.collider.transform.position, castDir);
        }
        return matchingTiles;
    }

    private void ClearMatch(Vector2[] paths)
    {
        List<GameObject> matchingTiles = new List<GameObject>();
        for (int i = 0; i < paths.Length; i++)
        {
            matchingTiles.AddRange(FindMatch(paths[i]));
        }
        if (matchingTiles.Count >= 2)
        {
            for (int i = 0; i < matchingTiles.Count; i++)
            {
                matchingTiles[i].GetComponent<SpriteRenderer>().sprite = null;
            }
            matchFound = true;
        }
    }

    public void ClearAllMatches()
    {
        if (render.sprite == null)
            return;

        ClearMatch(new Vector2[2] { Vector2.left, Vector2.right });
        ClearMatch(new Vector2[2] { Vector2.up, Vector2.down });
        if (matchFound)
        {
            render.sprite = null;
            matchFound = false;
            StopCoroutine(BoardManager.instance.FindNullTiles());
            StartCoroutine(BoardManager.instance.FindNullTiles());
            SFXManager.instance.PlaySFX(Clip.Clear);
        }
    }

    private bool HasMatch()
    {
        if (render.sprite == null)
            return false;

        List<GameObject> horizontalMatch = FindMatch(Vector2.left);
        horizontalMatch.AddRange(FindMatch(Vector2.right));
        List<GameObject> verticalMatch = FindMatch(Vector2.up);
        verticalMatch.AddRange(FindMatch(Vector2.down));

        return horizontalMatch.Count >= 2 || verticalMatch.Count >= 2;
    }
}
