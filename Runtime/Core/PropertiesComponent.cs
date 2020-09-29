using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MS.TweenAsync{


    public class PropertyVector2Accesser<TObject>:PropertyAccesser<TObject,UnityEngine.Vector2>{
        public readonly PropertyAccesser<TObject,float> x;
        public readonly PropertyAccesser<TObject,float> y;
        public PropertyVector2Accesser(Getter getter,Setter setter):base(getter,setter){
            x = new PropertyAccesser<TObject, float>(
                getter:(obj)=>{
                    return this.Get(obj).x;
                },
                setter:(obj,value)=>{
                    var v = this.Get(obj);
                    v.x = value;
                    this.Set(obj,v);
                }
            );
            y = new PropertyAccesser<TObject, float>(
                getter:(obj)=>{
                    return this.Get(obj).y;
                },
                setter:(obj,value)=>{
                    var v = this.Get(obj);
                    v.y = value;
                    this.Set(obj,v);
                }
            );
        }

    }

    public class PropertyVector3Accesser<TObject>:PropertyAccesser<TObject,UnityEngine.Vector3>{
        public readonly PropertyAccesser<TObject,float> x;
        public readonly PropertyAccesser<TObject,float> y;
        public readonly PropertyAccesser<TObject,float> z;
        public readonly PropertyAccesser<TObject,Vector2> xy;
        public readonly PropertyAccesser<TObject,Vector2> yz;
        public readonly PropertyAccesser<TObject,Vector2> xz;

        public PropertyVector3Accesser(Getter getter,Setter setter):base(getter,setter){
            x = new PropertyAccesser<TObject, float>(
                getter:(obj)=>{
                    return this.Get(obj).x;
                },
                setter:(obj,value)=>{
                    var v = this.Get(obj);
                    v.x = value;
                    this.Set(obj,v);
                }
            );
            y = new PropertyAccesser<TObject, float>(
                getter:(obj)=>{
                    return this.Get(obj).y;
                },
                setter:(obj,value)=>{
                    var v = this.Get(obj);
                    v.y = value;
                    this.Set(obj,v);
                }
            );

            z = new PropertyAccesser<TObject, float>(
                getter:(obj)=>{
                    return this.Get(obj).z;
                },
                setter:(obj,value)=>{
                    var v = this.Get(obj);
                    v.z = value;
                    this.Set(obj,v);
                }
            );

            xy = new PropertyAccesser<TObject, Vector2>(
                getter:(obj)=>{
                    return (Vector2)this.Get(obj);;
                },
                setter:(obj,value)=>{
                    var v = this.Get(obj);
                    v.x = value.x;
                    v.y = value.y;
                    this.Set(obj,v);
                }
            );
            yz = new PropertyAccesser<TObject, Vector2>(
                getter:(obj)=>{
                    var value = this.Get(obj);
                    return new Vector2(value.y,value.z);
                },
                setter:(obj,value)=>{
                    var v = this.Get(obj);
                    v.y = value.x;
                    v.z = value.y;
                    this.Set(obj,v);
                }
            );
            xz = new PropertyAccesser<TObject, Vector2>(
                getter:(obj)=>{
                    var value = this.Get(obj);
                    return new Vector2(value.x,value.z);
                },
                setter:(obj,value)=>{
                    var v = this.Get(obj);
                    v.x = value.x;
                    v.z = value.y;
                    this.Set(obj,v);
                }
            );
        }

    }
    public class PropertyVector4Accesser<TObject>:PropertyAccesser<TObject,UnityEngine.Vector4>{
        public readonly PropertyAccesser<TObject,float> x;
        public readonly PropertyAccesser<TObject,float> y;
        public readonly PropertyAccesser<TObject,float> z;

        public readonly PropertyAccesser<TObject,float> w;
        public PropertyVector4Accesser(Getter getter,Setter setter):base(getter,setter){
            x = new PropertyAccesser<TObject, float>(
                getter:(obj)=>{
                    return this.Get(obj).x;
                },
                setter:(obj,value)=>{
                    var v = this.Get(obj);
                    v.x = value;
                    this.Set(obj,v);
                }
            );
            y = new PropertyAccesser<TObject, float>(
                getter:(obj)=>{
                    return this.Get(obj).y;
                },
                setter:(obj,value)=>{
                    var v = this.Get(obj);
                    v.y = value;
                    this.Set(obj,v);
                }
            );

            z = new PropertyAccesser<TObject, float>(
                getter:(obj)=>{
                    return this.Get(obj).z;
                },
                setter:(obj,value)=>{
                    var v = this.Get(obj);
                    v.z = value;
                    this.Set(obj,v);
                }
            );
            w = new PropertyAccesser<TObject, float>(
                getter:(obj)=>{
                    return this.Get(obj).w;
                },
                setter:(obj,value)=>{
                    var v = this.Get(obj);
                    v.w = value;
                    this.Set(obj,v);
                }
            );
        }

    }

    public class PropertyColorAccesser<TObject>:PropertyAccesser<TObject,UnityEngine.Color>{
        public readonly PropertyAccesser<TObject,float> r;
        public readonly PropertyAccesser<TObject,float> g;
        public readonly PropertyAccesser<TObject,float> b;
        public readonly PropertyAccesser<TObject,float> a;
        public readonly PropertyVector3Accesser<TObject> rgb;


        public PropertyColorAccesser(Getter getter,Setter setter):base(getter,setter){
            r = new PropertyAccesser<TObject, float>(
                getter:(obj)=>{
                    return this.Get(obj).r;
                },
                setter:(obj,value)=>{
                    var v = this.Get(obj);
                    v.r = value;
                    this.Set(obj,v);
                }
            );
            g = new PropertyAccesser<TObject, float>(
                getter:(obj)=>{
                    return this.Get(obj).g;
                },
                setter:(obj,value)=>{
                    var v = this.Get(obj);
                    v.g = value;
                    this.Set(obj,v);
                }
            );

            b = new PropertyAccesser<TObject, float>(
                getter:(obj)=>{
                    return this.Get(obj).b;
                },
                setter:(obj,value)=>{
                    var v = this.Get(obj);
                    v.b = value;
                    this.Set(obj,v);
                }
            );

            a = new PropertyAccesser<TObject, float>(
                getter:(obj)=>{
                    return this.Get(obj).a;
                },
                setter:(obj,value)=>{
                    var v = this.Get(obj);
                    v.a = value;
                    this.Set(obj,v);
                }
            );

            rgb = new PropertyVector3Accesser<TObject>(
                getter:(obj)=>{
                    var color = this.Get(obj);
                    return new Vector3(color.r,color.g,color.b);
                },
                setter:(obj,value)=>{
                    var color = this.Get(obj);
                    color.r = value.x;
                    color.g = value.y;
                    color.b = value.z;
                    this.Set(obj,color);
                }
            );
        }

    }

}
