/* =====================================================================================
 * ARTiFICe - Augmented Reality Framework for Distributed Collaboration
 * ====================================================================================
 * Copyright (c) 2010-2012 
 * 
 * Annette Mossel, Christian Schönauer, Georg Gerstweiler, Hannes Kaufmann
 * mossel | schoenauer | gerstweiler | kaufmann @ims.tuwien.ac.at
 * Interactive Media Systems Group, Vienna University of Technology, Austria
 * www.ims.tuwien.ac.at
 * 
 * ====================================================================================
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *  
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *  
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 * =====================================================================================
 */
using UnityEngine;
using System.Collections;
using System;
using System.Text;
using System.Runtime.InteropServices;
//-----------------------------------------------------------------------------

[RequireComponent (typeof(Camera))]
public class VideoRenderer: MonoBehaviour {

    public Vector3 positionCamera = new Vector3(0, 0, -1);
    public Vector2 size=new Vector2(1,1);
    public bool renderInBack = true;
    public Rect normalizedViewPortRect=new Rect(0,0,1,1);
    public bool useDedicatedServer = false;
	private int imageScaleFactor=1;//Only works with DirectX - Consider removing
	private Texture2D tex;
	private Material imagePlaneMaterial;
    private int width = 0;
    private int height = 0;
    private uint m_updateCounter = 0;
    private GameObject videoObject;
    private GameObject backgroundCameraObject;
    private GameObject backgroundLightObject;
    private GameObject backgroundEraserObject;
    private MeshFilter videoMesh;
    private MeshRenderer videoMeshRenderer;
    private bool videoInitialized = false;
    VideoSubscriber videoSubscriber;
    //-------------------------------------------------------------------------

    void Start()
    {
        Manager.useDedicatedServer = useDedicatedServer;
        if (!Manager.useDedicatedServer)
        {
            StartVideoRenderer();
        }
    }

    void OnConnectedToServer()
    {
        if (Network.isClient)
        {
            StartVideoRenderer();
        }
    }

    void Update()
    {
        if (!Manager.useDedicatedServer || Network.isClient)
        {
            UpdateVideoRenderer();
        }
    }
	void StartVideoRenderer () 
    {
           videoSubscriber = Manager.instance.video.getVideoSubscriber("video");
	}

    void UpdateVideoRenderer()
    {
        if (!useDedicatedServer || !Network.isServer)
        {
            VideoFrame videoFrame = new VideoFrame();
            //Initialize Videoplane, BackgroundCamera etc.
            if (!videoInitialized)
            {
                if (videoInitialized = videoSubscriber.getPixels(videoFrame))
                {
                    width = videoFrame.getWidth();
                    height = videoFrame.getHeight();

                    videoObject = new GameObject("VideoObject");
                    videoMesh = videoObject.AddComponent<MeshFilter>();
                    float horScale = 1.0f;// width;
                    float verScale = 1.0f;//height;
                    Vector3[] newVertices = { new Vector3(0, 0, 0), new Vector3(0, verScale, 0), new Vector3(horScale, verScale, 0), new Vector3(horScale, 0, 0) };
                    Vector2[] newUV = { new Vector2(1, 1), new Vector2(1, 0), new Vector2(0, 0), new Vector2(0, 1) };
                    int[] newTriangles = { 0, 1, 2, 2, 3, 0 };

                    videoMesh.mesh.vertices = newVertices;
                    videoMesh.mesh.uv = newUV;
                    videoMesh.mesh.triangles = newTriangles;
                    videoMesh.mesh.RecalculateNormals();

                    videoMeshRenderer = videoObject.AddComponent<MeshRenderer>();
                    videoObject.layer = 10;
                    videoMeshRenderer.enabled = true;
                    imagePlaneMaterial = new Material(Shader.Find("Diffuse"));
                    videoMeshRenderer.material = imagePlaneMaterial;


                    tex = new Texture2D(width/imageScaleFactor, height/imageScaleFactor, TextureFormat.RGB24, false);//new Texture2D(width, height);	
                    this.imagePlaneMaterial.SetTexture("_MainTex", tex);

                    //flip y-Axis
                    Vector3 _positionCamera=positionCamera;
                    _positionCamera.y = -_positionCamera.y;

                    createForeGroundCamera(_positionCamera, size);
                    createBackGroundCamera(_positionCamera, size);
                    createVideoPlane(_positionCamera, size);
                }
            }
            //Update the texture every time there is a new video-frame
            if (videoInitialized)
            {
                uint currentCounter = videoSubscriber.getUpdateCounter();
                if (m_updateCounter != currentCounter)
                {
                    m_updateCounter = currentCounter;
                    videoSubscriber.getPixels(videoFrame);
					int texId=tex.GetNativeTextureID();
					if (! videoFrame.set(texId))//Works only in OpenGL 
					{
	                    Color[] val = new Color[width/imageScaleFactor * height/imageScaleFactor];
					
                   		float[] color = new float[3];
	                    for (int y = 0; y < height; y++)
	                    {
	                        for (int x = 0; x < width; x++)
	                        {
	                            if ((y < height) && (x < width))
	                            {
	                                videoFrame.getPixel(x, y, out color[0], out color[1], out color[2]);
	                            }
	                            else
	                            {
	                                color[0] = 0.0f;
	                                color[1] = 0.0f;
	                                color[2] = 0.0f;
	                            }
								val[(y/imageScaleFactor * width/imageScaleFactor) + x/imageScaleFactor] = new Color(color[0], color[1], color[2]);
	                            //val[y * width + x] = new Color(color[0], color[1], color[2]);
	                        }
	                    }
	
	                    tex.SetPixels(val);
	                    tex.Apply();
					}
                }
            }
        }
	}

	//-------------------------------------------------------------------------
	
	public void OnApplicationQuit(){
	}

    // left/right/top/bottom define near plane size, i.e.
    // how offset are corners of camera's near plane.
    private Matrix4x4 PerspectiveOffCenter(
        float left,float right,
        float bottom,float top,
        float near , float far)
    {
        float x = (2.0f * near) / (right - left);
        float y = (2.0f * near) / (top - bottom);
        float a = (right + left) / (right - left);
        float b = (top + bottom) / (top - bottom);
        float c = -(far + near) / (far - near);
        float d = -(2.0f * far * near) / (far - near);
        float e = -1.0f;

        Matrix4x4 m=new Matrix4x4();
        m[0,0] = x; m[0,1] = 0; m[0,2] = a; m[0,3] = 0;
        m[1,0] = 0; m[1,1] = y; m[1,2] = b; m[1,3] = 0;
        m[2,0] = 0; m[2,1] = 0; m[2,2] = c; m[2,3] = d;
        m[3,0] = 0; m[3,1] = 0; m[3,2] = e; m[3,3] = 0;
        return m;
    }

    private void SetOffAxisCamera(Camera currentCam,Vector3 position,Vector2 size)
    {
        //invert
        position = position * -1.0f;

        float nearPlane = currentCam.nearClipPlane;
        float farPlane = currentCam.farClipPlane;

        // left/right/top/bottom define near plane size, i.e.
        // how offset are corners of camera's near plane.
        float left =-size.x / 2.0f+position.x;
        float right=size.x /2.0f+position.x;
        float bottom=-size.y /2.0f+position.y;
        float top=size.y /2.0f+position.y;

        currentCam.projectionMatrix = PerspectiveOffCenter(left * nearPlane, right * nearPlane, bottom * nearPlane, top * nearPlane, nearPlane, farPlane);
        
    }

    private void createVideoPlane(Vector3 position,Vector2 size)
    {
        //invert
        position = position * -1.0f;
        
        //scales the videoplane to the right size, so it fills the screen on a distance of 1
        Vector3 locScale = new Vector3(size.x , size.y, 1.0f);
        //position of the videoplane -the calibrated camera pos on z and offset on xy by calibration
        Vector3 locPos = new Vector3(-size.x / 2.0f + position.x, -size.y / 2.0f + position.y, +position.z);

        videoObject.transform.parent = transform; //pitfall!
        videoObject.transform.localPosition = locPos;
        videoObject.transform.localScale=locScale;
		videoObject.transform.localRotation = Quaternion.identity; //pitfall!
        videoObject.hideFlags |= HideFlags.HideInHierarchy;
		videoObject.hideFlags |= HideFlags.HideInInspector;
		videoObject.hideFlags |= HideFlags.NotEditable;
    }
    
    private void createForeGroundCamera(Vector3 position, Vector2 size)
    {
        gameObject.camera.clearFlags = CameraClearFlags.Depth;
        gameObject.camera.backgroundColor = new Color(0.1f, 0.1f, 0.1f);

        //set position in renderqueue
        if (this.renderInBack)
        {
            gameObject.camera.depth = -98;//almost smallest possible depth, is rendered behind everything after deleting things from beforeframe
        }
        else
        {
            gameObject.camera.depth = 100;//biggest possible depth, is rendered in front of everything
        }

        //set layer
        //set foreground camera, so that it doesn't render the videoplane
        gameObject.camera.cullingMask = ~(1 << 10);//make main camera not render layer 10

        //set paramters for hte offaxis camera
        this.SetOffAxisCamera(gameObject.camera, position, size);

        //set renderwindow on the screen
        gameObject.camera.rect = normalizedViewPortRect;


    }

    private void createBackGroundCamera(Vector3 position, Vector2 size)
    {
        //Create gameobject and attach camera
        backgroundCameraObject = new GameObject("BackgroundCameraObject");
        backgroundCameraObject.AddComponent<Camera>();
		backgroundCameraObject.transform.parent = transform; //pitfall!
		backgroundCameraObject.transform.localPosition = Vector3.zero; //pitfall!
		backgroundCameraObject.transform.localRotation = Quaternion.identity;
		backgroundCameraObject.transform.localScale = Vector3.one;
        Camera bGCamera = backgroundCameraObject.camera;

        //set basic parameters
        bGCamera.nearClipPlane = 0.01f;
        bGCamera.farClipPlane = 1.1f * transform.lossyScale.z;
        if (bGCamera.farClipPlane < 10000.0f)
        {
            bGCamera.farClipPlane = 10000.0f;
        }
        bGCamera.clearFlags = CameraClearFlags.SolidColor;
        bGCamera.backgroundColor = new Color(0.1f, 0.1f, 0.1f);

        //set position in renderqueue
        if (this.renderInBack)
        {
            bGCamera.depth = -99;//almost smallest possible depth, is rendered behind everything after deleting things from beforeframe
        }
        else
        {
            bGCamera.depth = 99;//biggest possible depth, is rendered in front of everything
        }

        //set layer
        bGCamera.cullingMask = 1 << 10;//layer 10 choosen arbitrary
        gameObject.camera.cullingMask = ~(1 << 10);//make main camera not render layer 10

        //hideflags set to not visible in hierarchy and non modifyable
        backgroundCameraObject.hideFlags |= HideFlags.HideInHierarchy;
        backgroundCameraObject.hideFlags |= HideFlags.HideInInspector;
        backgroundCameraObject.hideFlags |= HideFlags.NotEditable;

        //set paramters for hte offaxis camera
        this.SetOffAxisCamera(bGCamera, position, size);

        //set renderwindow on the screen
        bGCamera.rect=normalizedViewPortRect;

        //Put in a light that just shines for us
        backgroundLightObject = new GameObject("BackgroundLightObject");
	
		backgroundLightObject.transform.parent = transform; //pitfall!
		backgroundLightObject.transform.localPosition = new Vector3(0.0f, 0.0f, -3.0f); //pitfall!
		backgroundLightObject.transform.localRotation = Quaternion.identity;
		backgroundLightObject.transform.localScale = Vector3.one;
        backgroundLightObject.AddComponent<Light>();
        backgroundLightObject.light.cullingMask = 1 << 10;//layer 10 same as videoplane
        backgroundLightObject.hideFlags |= HideFlags.HideInHierarchy;
        backgroundLightObject.hideFlags |= HideFlags.HideInInspector;
        backgroundLightObject.hideFlags |= HideFlags.NotEditable;

        //Put in a camera to delete the things from last frame Create gameobject and attach camera
        backgroundEraserObject = new GameObject("BackgroundEraserObject");
        backgroundEraserObject.AddComponent<Camera>();
        backgroundEraserObject.camera.depth = -100;
        backgroundEraserObject.camera.clearFlags = CameraClearFlags.SolidColor;
        backgroundEraserObject.camera.cullingMask = 0;
        //hideflags set to not visible in hierarchy and non modifyable
        backgroundEraserObject.hideFlags |= HideFlags.HideInHierarchy;
        backgroundEraserObject.hideFlags |= HideFlags.HideInInspector;
        backgroundEraserObject.hideFlags |= HideFlags.NotEditable;
    }
}

