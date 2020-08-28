# UniTweenAsync

Async Tween Function Extensions.

* async/await api design
* free allocation
* ease to extend

# Dependencies

* [LitTask - Free Allocation Task-Like Implementation](https://github.com/wlgys8/LitTask)

* [MSAsync - Unity Awaiters Implementation](https://github.com/wlgys8/MSAsync)


# Standard Implementation

* All async tween function will return `TweenOperation`.
* Use `TweenOptions` to config the tween animation.


## TweenOperation

- `async/await`

```csharp

async void Start(){
    //move to position (100,100,100) and wait the animation complete
    var operation = gameObject.MoveToAsync(new MoveToOptions(new Vector3(100,100,100)));
    //use Task for await
    await operation.Task;
    //animation completed.
}

```

- `Control tween state`

```csharp

private TweenOperation operation;

void Start(){
    operation = gameObject.MoveToAsync(new MoveToOptions(new Vector3(100,100,100)));
}

///pause tween animation
void PauseTween(){
    operation.paused = true;
}


///resume tween animation
void ResumeTween(){
    operation.paused = false;
}

///cancel tween animation
void CancelTween(){
    operation.Cancel();
}

//locate the tween animation to last frame.
void RanToEnd(){
    operation.RanToEnd();
}

```

## TweenOptions

- `duration` - how much time does the animation take. 
- `ignoreTimeScale` should the animation affected by Time.timeScale
- `ease` - ease function for animation. Default is Linear.

```csharp

void Start(){
    gameObject.MoveToAsync(
        new MoveToOptions(){
            position = new Vecotor3(100,100,100),
            tweenOptions = new TweenOptions(){
                duration = 0.5f,
                ease = EaseFuncs.OutBack,
                ignoreTimeScale = true,
            }
        }
    )
}

```

# Async Tween Functions List

## Transform

- `MoveToAsync`
- `MoveByAsync`
- `ScaleToAsync`
- `RotateToAsync`

## UI.Graphic

- `TintToAsync`
- `AlpahToAsync`

## CanvasGroup

- `AlphaToAsync`

## RectTransform

- `AnchoredPositionToAsync`
- `SizeToAsync`


# TweenComposite

- TweenComposite.Sequence
- TweenComposite.Parallel


# ValueToAsync

TweenAsync support run custom interpolation function between two State.

```csharp

//duration is 1 second.
var tweenOptions = new TweenOptions(1);

//update function.
//value will change from 0 to 100
Action<float> update = (value)=>{
    Debug.Log("current value:" + value);
};

new ValueToOptions<float>(0,100,interpolation,tweenOptions).RunAsync();

```

Builtin supported types: 
- float
- double
- int
- Color
- Vector2
- Vector3
- Vector4
- Quaternion