using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Wire : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
   private Image image;
   public Color customColor;   
   private LineRenderer lineRenderer;
   private Canvas canvas;
   private bool isDragStarted;
   public bool isLeftWire;
   public bool isSuccess;
   
   private TaskFixWiring taskFixWiring;

   public void Awake()
   {
      image = GetComponent<Image>();
      lineRenderer = GetComponent<LineRenderer>();
      canvas = GetComponentInParent<Canvas>();
      taskFixWiring = GetComponentInParent<TaskFixWiring>();
   }

   private void Update()
   {
      if (isDragStarted)
      {
         Vector2 movePos;
         RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, Input.mousePosition,
            canvas.worldCamera, out movePos);
         
         lineRenderer.SetPosition(0, transform.position);
         lineRenderer.SetPosition(1, canvas.transform.TransformPoint(movePos));
      }
      else
      {
         //Hide the line if not dragging. We will not hide if when it connects
         if(!isSuccess)
         {
            lineRenderer.SetPosition(0, Vector3.zero);
            lineRenderer.SetPosition(1, Vector3.zero);
         }
      }
      
      bool isHovered = RectTransformUtility.RectangleContainsScreenPoint(transform as RectTransform, Input.mousePosition, canvas.worldCamera);

      if (isHovered)
         taskFixWiring.currentHoveredWire = this;
   }

   public void SetColor(Color color) 
   {
      image.color = color;
      lineRenderer.startColor = color;
      lineRenderer.endColor = color;
      customColor = color;
   }

   public void OnDrag(PointerEventData eventData)
   {
      //needed for drag but not used
   }

   public void OnBeginDrag(PointerEventData eventData)
   {
      if(!isLeftWire)
         return;
      //if it is successful, don't draw lines
      if(isSuccess)
         return;
      isDragStarted = true;
      taskFixWiring.currentDraggedWire = this;
   }

   public void OnEndDrag(PointerEventData eventData)
   {
      if (taskFixWiring.currentDraggedWire != null)
      {
         if (taskFixWiring.currentHoveredWire.customColor == customColor && !taskFixWiring.currentHoveredWire.isLeftWire)
         {
            isSuccess = true;
            //set successful on the right wire as well
            taskFixWiring.currentHoveredWire.isSuccess = true;
         }
      }
      
      isDragStarted = false;
      taskFixWiring.currentDraggedWire = null;
   }
}