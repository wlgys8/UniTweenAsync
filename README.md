# UniTweenAsync

High performance tween animation framework for Unity

Unity下的高性能Tween动画框架

* free allocation 无GC
* high performance 高性能
* support async/await 支持async/await异步等待
* ease to extend 非常方便的自定义扩展
* support run in editor mode 支持在编辑器模式下运行

# Dependencies

* [LitTask - Free Allocation Task-Like Implementation](https://github.com/wlgys8/LitTask)

# Table of Content
- Quick Example (快速范例)

- Basic Usage (基础)
    - Basic Conception (基础概念)
        - Interpolation Model 数据插值模型
        - Tweenable Property 目标属性
        - TweenOptions 动画基础配置
    - Control Tween Status (控制动画状态)
    - Delay (延迟)
    - Composite Tweens (动画组合)
    - Callback (回调函数)
    - Binding with GameObject (绑定GameObject)    
- Advanced Usage (进阶)
    - Property Component (属性分量)
    - Complex Properties (复杂属性)
        - Circular Motion & Degree Property (圆周运动与角度属性)
    - Custom 自定义扩展
        - Custom Interpolatable ValueType 自定义插值数据类型
            - lerp operation 插值运算
            - distance operation 距离运算
        - Custom Tweenable Property 自定义目标属性
        - Custom EaseFunc 自定义动画曲线
- API Extensions (API扩展)


# Quick Example

This is an example that move object from (0,0,0) to (100,100,100) in 2 seconds with EaseOutBack. 
0 gc alloc.

以下代码示范了本框架的基本使用方式. 
例子为将目标对象从(0,0,0)移动到(100,100,100), 花费时间2秒, 动画曲线为OutBack。 该动画无任何GC开销

```csharp

Values.Range(
    new Vector3(0,0,0), //from
    new Vector3(100,100,100) //to
).Property(
    transform, //target
    Properties.transform.localPosition //property
    new TweenOptions(){
        duration = 2, //2 seconds
        ease = EaseFuncs.OutBack //ease func
    },
);

```

# Basic Usage (基础)

## Basic Conception

基本的Tween动画由三个部分定义而成，即数据插值模型、目标属性、动画基础配置.


### 数据插值模型

数据插值当前有五种模式: 

- Range 指定一个启始数值F和目标数值T, 插件区间为[F,T]
- To 指定一个目标数值T,插值区间为[当前,T]
- From 指定一个启始数值F，插值区间为[F,当前]
- By 指定一个变化值D,插值区间为[当前,当前+D]
- Sequence 序列模式,相当于多区间插值. 数据会从头匀速变化到尾.
    - 注意,由于涉及到速度概念，使用Sequence模式时，数据类型必须支持distance运算. 

内置支持的数据类型有 `float`,`int`,`double`,`Vector2`,`Vector3`,`Vector4`,`Color`,
`Quaternion`

```csharp

//这样即可以定义从点(0,0,0)到点(100,100,0)的一个数据插值对象
new Range<Vector3>(new Vector3(0,0,0),new Vector3(100,100,100));

//另一种写法
Values.Range(new Vector3(0,0,0),new Vector3(100,100,100));


```

### 目标属性

目标属性定义了数据插值要驱动的对象. 其基本类型为 `PropertyAccesser<TObject,TValue>`.

例如我们要驱动`Transform`的`localPosition`属性，那么可以定义一个如下的对象:

```csharp

public static class Properties{
    public static readonly PropertyAccesser<UnityEngine.Transform,Vector3> localPosition = new PropertyAccesser<UnityEngine.Transform, Vector3>(
        //set方法
        setter:(transform,localPosition)=>{
            transform.localPosition = localPosition;
        },
        //get方法
        getter:(transform)=>{
            return transform.localPosition;
        }
    );
}
```

然后我们用Range去驱动这个属性:

```csharp

Values.Range(new Vector3(0,0,0),new Vector3(100,100,100)).Property(transform,Properties.localPosition);

```

即可执行成将transform从(0,0,0)移动到(100,100,100)的动画.

#### 内置的插值属性

- Properties
    - transform
        - localPosition
        - localScale
        - localEulerAngles
        - localRotation
        - position
        - eulerAngles
        - rotation
    - rectTransform
        - anchoredPosition
        - size
    - graphic
        - alpha
        - color
    - canvasGroup
        - alpha

### TweenOptions 动画基础配置

`TweenOptions` provide the common properties for tweens.

`TweenOptions` 定义了Tween动画共享的基础配置

- `duration` [1]  

    how much time does the animation take.  (动画花费的时间)
- `ignoreTimeScale` [false] 

    should the animation affected by Time.timeScale 
    (是否受Time.timeScale影响)
- `ease` [EaseFunc.Linear] 

     ease function for animation. (Tween动画Ease函数)

## Control Tween Status (控制动画状态)


All the Tween Function will return `TweenOperation`, and we can use it to do somethings as follows:

- Read tween's status.
- Pause/Resume the tween.
- Cancel the tween.
- Run the tween to the end immediately.
- await the tween in async function.

### Usage
- `async/await`

```csharp

async void Start(){
    //move to position (100,100,100) and wait the animation complete
    var operation = Values.To(
        new Vector3(100,100,100)
    ).Property(
        transform, 
        Properties.transform.localPosition
    );
    //use Task for await
    await operation.Task;
    //animation completed.
}

```

- `Change Tween Status`

```csharp

private TweenOperation operation;

void Start(){
    operation = Values.To(
        new Vector3(100,100,100)
    ).Property(
        transform, 
        Properties.transform.localPosition
    );
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

## Delay (延迟)

TweenOperation.Delay(float seconds);

## Composite Tweens (动画组合)

- TweenOperation.Sequence 
- TweenOperation.Parallel
- TweenOperation.Repeat
- TweenOperation.RepeatForever

## Callback

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

## Binding with GameObject 绑定GameObject

有时候，我们希望将Tween动画与一个gameObject的生命周期作绑定。
例如
- 当gameObject的`[active=false]`时，就暂停动画,[`active=true`]时继续动画
- 当gameObject销毁时，同步销毁动画

框架提供了一个扩展方法来实现这个功能:

```csharp

static TweenOperation Bind(this TweenOperation operation,GameObject go, BehaviourBindSetting setting);

```

`BehaviourBindSetting` 提供如下字段,类型均为`BehaviourType`枚举:

- `behaviourOnEnable` -  OnEnable时要执行的行为
- `behaviourOnDisable` - OnDisable时要执行的行为
- `behaviourOnDestroy` - OnDestroy时要执行的行为

BehaviourType 有以下行为:

- None - 什么都不做
- RanToEnd - 定位到动画结尾
- Cancel - 取消动画
- Resume - 继续动画
- Pause - 暂停动画


使用方式:

```csharp

void Foo(){
    var operation = Values.To(
        new Vector3(100,100,100)
    ).Property(
        transform, 
        Properties.transform.localPosition
    ).Bind(
        gameObject,
        new BehaviourBindSetting(){
            //OnDisable的时候，暂停动画
            behaviourOnDisable = BehaviourType.Pause,
            //OnEnable的时候，继续动画
            behaviourOnEnable = BehaviourType.Resume,
            //OnDestroy的时候，取消动画
            behaviourOnDestroy = BehaviourType.Cancel
        },
        //这也是BehaviourBindSetting.Default的默认实现方式
    );
}

```

### `Attention(注意):`

由于要监听gameObject的生命周期，Bind行为会在指定的GameObject上创建并绑定一个MonoBehaviour(仅第一次绑定的时候)，此处是有GC Alloc开销的。
所以通常不宜大量频繁的往不同的GameObject上执行Bind行为。

# Advanced Usage (进阶)

## Property Component (属性分量)

言外之意就是我们可以针对某个属性的其中一个分量进行动画。例如

```csharp

Values.To(100f).Property(
    transform,
    Properties.transform.localPosition.x //此处我们只针对x分量，将其从0变化到100
);

```

目前支持属性分量的类型有:
- `PropertyVector2Accesser<TObject>`
    - x
    - y
- `PropertyVector3Accesser<TObject>`
    - x
    - y
    - z
    - xy
    - yz
    - xz
- `PropertyColorAccesser<TObject>`
    - r
    - g
    - b
    - a
    - rgb




## Complex Properties (复杂属性)

利用属性的基本概念，我们实际上可以还可以创建一些复杂的属性。 以`PropertiesUtility`中内置实现的几个方法为例:

### 1. 圆周运动和角度属性

假如我们希望实现物体围绕某个圆心c，以半径r作圆周运动，我们就可以利用角度属性来实现。
以`PropertiesUtility`中的内置实现为例， 下面的代码定义了一个函数，可以将位置属性(`PropertyAccesser<TObject,Vector2>`)转为以`center`为圆心,`radius`为半径的圆周空间内的角度属性(`PropertyAccesser<TObject,float>`).

```csharp

///将位置属性(position)转换为以(center)为圆心，(radius)为半径的角度属性
public static PropertyAccesser<TObject,float> Degree<TObject>(PropertyAccesser<TObject,Vector2> position,Vector2 center,float radius){
    return new PropertyAccesser<TObject, float>(
        setter:(obj,value)=>{
            var x = Mathf.Cos(value * Mathf.Deg2Rad);
            var y = Mathf.Sin(value * Mathf.Deg2Rad);
            position.Set(obj,radius * new Vector2(x,y));
        },
        getter:(obj)=>{
            var pos = position.Get(obj);
            var vec = pos - center;
            return Vector2.SignedAngle(Vector2.right,vec);
        }
    );
}

```

圆周动画本质是将角度从0变到360。因此我们可以按如下方式驱动:

```csharp

//将位置属性(anchoredPosition)变换为一个角度属性，以(0,0)为圆心，100为半径 (注意此处有alloc)
var degree = PropertiesUtility.Degree(Properties.rectTransform.anchoredPosition,Vector2.zero,100);

//执行从0到360的角度动画
Values.Range(0f,360f).Property(rectTransform,degree);

```


## Custom (自定义扩展)

### Custom Interpolatable ValueType 自定义插值数据类型

#### 1. 插值运算

插值运算可以分解为加法运算和乘法元素，可以通过`ValueUtility.ImplLerp<YOUR_VALUE_TYPE>`接口来为自定义类型实现插值运算支持.

```csharp
    ValueUtility.ImplLerp<YOUR_VALUE_TYPE>(
        addFunc:(v1,v2)=>{
            //return add(v1 , v2);
        },
        mulFunc:(value,t)=>{
            //return mul(value,t);
        }
    );
```

#### 2. 距离运算

部分动画需要数据支持距离运算，可以通过`ValueUtility.ImplDistance<YOUR_VALUE_TYPE>` 来实现.

```csharp

ValueUtility.ImplLerp<YOUR_VALUE_TYPE>((v1,v2)=>{
    float distance = 0;
    //....calculate distance...
    return distance;
});

```

### Custom Tweenable Property 自定义目标属性

在任意地方实现一个`PropertyAccesser<TObject,TValue>`类型的对象, 其中

- TObject为属性所属对象的类型
- TValue为属性的数值类型

例如`Properties.transform.localPosition`的实现代码如下:

```csharp

public static readonly PropertyAccesser<UnityEngine.Transform,Vector3> localPosition = new PropertyAccesser<UnityEngine.Transform, Vector3>(
    setter:(transform,localPosition)=>{
        transform.localPosition = localPosition;
    },
    getter:(transform)=>{
        return transform.localPosition;
    }
);
```



### Custom EaseFunc 自定义动画曲线

EaseFunc是一个delegate类型， 我们可以提供任意的EaseFunc对象来作为动画曲线. 参数t的取值范围为[0,1]. 

例如线性动画曲线如下:

```csharp

public static readonly EaseFunction Linear = (float t)=>{
    return t;
};

```


# API Extensions

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