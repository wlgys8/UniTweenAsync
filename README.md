# UniTweenAsync

Async Tween Function Extensions.

* async/await api design
* free allocation
* ease to extend

# Dependencies

* [LitTask - Free Allocation Task-Like Implementation](https://github.com/wlgys8/LitTask)

* [MSAsync - Unity Awaiters Implementation](https://github.com/wlgys8/MSAsync)


# Usage

* All async function implemented by Extensions. 
* (optional) - Use `TweenOperation` to control the animation state. 

## GameObject Tween for Example

```csharp

//use tween operation to control the animation state
private TweenOperation operation = new TweenOperation();

async void Start(){
    //move to position (100,100,100) and wait the animation complete
    await gameObject.MoveToAsync(new MoveToOptions(new Vector3(100,100,100)));
    //then, scale to (2,2,2) and forget it
    gameObject.ScaleToAsync(new ScaleToOptions(new Vector3(2,2,2))).Forget();
    
    //rotate forever
    RotateForever().Forget();
}

private LitTask RotateForever(){
    try{
        while(true){
            //use tween operation to control the animation state
            await gameObject.RotateToAsync(new RotateToOptions(new Vector3(360,0,0)),operation);
        }
    }catch(LitCancelException){
        //operation.Cancel will abort the tween and throw LitCancelException
    }catch(Exception){
        //other exceptions
    }
}

void OnEnable(){
    //if script enabled, then resume the rotate task
    operation.paused = false;
}

void OnDisable(){
    //if script disabled, then pause the rotate task
    operation.paused = true;
}

void OnDestroy(){
    //cancel the task
    operation.Cancel();
}

```


# TO BE CONTINUE