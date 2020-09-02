# UniTweenAsync

Async Tween Function Extensions.

* free allocation
* support async/await
* ease to extend
* support run in editor mode

# Dependencies

* [LitTask - Free Allocation Task-Like Implementation](https://github.com/wlgys8/LitTask)

# Table of Content

- Tween Functions List
    - Transform
    - UI.Graphic
    - CanvasGroup
    - RectTransform

- TweenOptions
- TweenOperation
- Composite Tweens
- Callback
- Custom Tween Functions
- ValueToAsync





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


# TweenOptions

TweenOptions provide the common properties for tweens.

- `duration` - how much time does the animation take. 
- `ignoreTimeScale` should the animation affected by Time.timeScale
- `ease` - ease function for animation. Default is Linear.

```csharp

void Start(){
    gameObject.MoveToAsync(
        new MoveToOptions(){
            position = new Vecotor3(100,100,100),
            //usage of tweenOptions in MoveToAsync
            tweenOptions = new TweenOptions(){
                duration = 0.5f,
                ease = EaseFuncs.OutBack,
                ignoreTimeScale = true,
            }
        }
    )
}

```

# TweenOperation

All Async Tween Function should return `TweenOperation`, and we can use it to do somethings like below:

- Read tween's status.
- Pause/Resume the tween.
- Cancel the tween.
- Run the tween to the end immediately.
- await the tween in async function.

## Usage
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

- `Control The Tween`

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






# Composite Tween

- TweenOperation.Sequence 
- TweenOperation.Parallel
- TweenOperation.Repeat
- TweenOperation.RepeatForever


# Callbacks

- TweenOperation.Callback

```csharp

void Start(){
    TweenOperation.Sequence(
        TweenOperation.Callback(()=>{
            //called before move start
        }),
        gameObject.MoveToAsync(....),

        TweenOperation.Callback(()=>{
            //called after move finished
        }),
    );
}

```

# Custom Tween Functions

To be written

# ValueToAsync

TweenAsync support run custom interpolation function between two state.

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