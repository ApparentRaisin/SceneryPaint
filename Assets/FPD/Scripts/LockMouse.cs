// by @torahhorse

using UnityEngine;
using System.Collections;

public class LockMouse : MonoBehaviour
{	
	void Start()
	{
		LockCursor(true);
		PaintModeController.paintingToggle.AddListener(SetCursor);
	}
	
	void SetCursor(){
		LockCursor(!PaintModeController.isPainting);
	}

    void Update()
    {
    	// lock when mouse is clicked
    	if( Input.GetMouseButtonDown(0) && Time.timeScale > 0.0f && !PaintModeController.isPainting)
    	{
    		LockCursor(true);
    	}
    
    	// unlock when escape is hit
        if  ( Input.GetKeyDown(KeyCode.Escape) )
        {
        	LockCursor(!Screen.lockCursor);
        }
    }
    
    public void LockCursor(bool lockCursor)
    {
    	Screen.lockCursor = lockCursor;
    }
}