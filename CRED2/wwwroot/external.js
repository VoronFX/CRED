//import "react";
//import "react-dom";
//import "jquery";
//import "semantic/semantic";

// ReSharper disable ExperimentalFeature
import("react")
    .then(() => import("react"))
    .then(() => import("react-dom"))
    .then(() => import("jquery"))
    .then(() => import("semantic/semantic"))
    .then(() => import("Bridge.NET/bridge.js"))
    .then(() => import("Bridge.NET/bridge.console.js"))
    .then(() => import("Bridge.NET/bridge.meta.js"))
    .then(() => import("Bridge.NET/ProductiveRage.Immutable.js"))
    .then(() => import("Bridge.NET/ProductiveRage.Immutable.meta.js"))
    .then(() => import("Bridge.NET/Bridge.React.js"))
    .then(() => import("Bridge.NET/Bridge.React.meta.js"))
    //.then(() =>import("Bridge.NET/jquery-2.2.4.js"))
    .then(() => import("Bridge.NET/productiveRage.immutable.extensions.js"))
    .then(() => import("Bridge.NET/productiveRage.immutable.extensions.meta.js"))
    .then(() => import("Bridge.NET/bridge.js"));

//    .then(()=>import("Bridge.NET/bridge.js"))
//import("Bridge.NET/bridge.console.js");
//import("Bridge.NET/bridge.meta.js");
//import("Bridge.NET/ProductiveRage.Immutable.js");
//import("Bridge.NET/ProductiveRage.Immutable.meta.js");
//import("Bridge.NET/Bridge.React.js");
//import("Bridge.NET/Bridge.React.meta.js");
////import("Bridge.NET/jquery-2.2.4.js");
//import("Bridge.NET/productiveRage.immutable.extensions.js");
//import("Bridge.NET/productiveRage.immutable.extensions.meta.js");
//
// ReSharper restore ExperimentalFeature