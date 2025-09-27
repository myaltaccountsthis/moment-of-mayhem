using UnityEngine;
using UnityEngine.UI;

public class PlayerSelector : MonoBehaviour
{
    // to add: player, gamecontroller, selection box
    public Entity selectedEntity;


    void Update()
    {
        Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mouseWorldPos, Vector2.zero);

        // Check if the ray hit a collider
        Entity oldEntity = selectedEntity;
        if (hit.collider != null)
        {
            GameObject hoveredObject = hit.collider.gameObject;
            Entity entity = hoveredObject.GetComponent<Entity>();
            selectedEntity = entity;
            UpdateSelectionOutline(oldEntity);
            if (entity != null)
            {
                // The hovered object has an Entity component
                // You can now interact with the entity as needed
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    // Call interact on entity
                }
            }
            Debug.Log("Mouse is hovering over: " + hoveredObject.name);
        }
        else
        {
            selectedEntity = null;
        }
    }

    // Selected entity should have an outline component, should be updated to new one
    void UpdateSelectionOutline(Entity oldEntity)
    {
        if (selectedEntity == oldEntity)
        {
            return;
        }

        if (oldEntity != null)
        {
            // Deselect logic here, remove render
            Debug.Log("Deselected entity: " + oldEntity.name);
            if (oldEntity.GetComponent<Outline>() != null)
            {
                oldEntity.GetComponent<Outline>().enabled = false;
            }
        }

        // Update UI or other game elements to reflect the new selection
        Debug.Log("Selected entity: " + (selectedEntity != null ? selectedEntity.name : "None"));
        // Implement select logic
        if (selectedEntity != null)
        {
            if (selectedEntity.GetComponent<Outline>() != null)
            {
                selectedEntity.GetComponent<Outline>().enabled = true;
            }
        }
    }
}
