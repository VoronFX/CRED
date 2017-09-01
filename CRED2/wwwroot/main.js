
// ReSharper disable ExperimentalFeature
import(/* webpackChunkName: "react.js" */ `react`)
    .then(() => import(/* webpackChunkName: "react-dom.js" */ "react-dom"))
    .then(() => import(/* webpackChunkName: "jquery.js" */ "jquery"))
    .then(() => import(/* webpackChunkName: "semantic.js" */ "semantic/semantic"))
    .then(() => import(/* webpackChunkName: "bridge.js" */ "Bridge.NET/bridge.js"))
    .then(() => import(/* webpackChunkName: "bridge.console.js"" */ "Bridge.NET/bridge.console.js"))
    .then(() => import(/* webpackChunkName: "bridge.meta.js" */ "Bridge.NET/bridge.meta.js"))
    .then(() => import(/* webpackChunkName: "ProductiveRage.Immutable.js" */ "Bridge.NET/ProductiveRage.Immutable.js"))
    .then(() => import(/* webpackChunkName: "ProductiveRage.Immutable.meta.js" */ "Bridge.NET/ProductiveRage.Immutable.meta.js"))
    .then(() => import(/* webpackChunkName: "Bridge.React.js" */ "Bridge.NET/Bridge.React.js"))
    .then(() => import(/* webpackChunkName: "Bridge.React.meta.js" */ "Bridge.NET/Bridge.React.meta.js"))
    .then(() => import(/* webpackChunkName: "productiveRage.immutable.extensions.js" */ "Bridge.NET/productiveRage.immutable.extensions.js"))
    .then(() => import(/* webpackChunkName: "productiveRage.immutable.extensions.meta.js" */ "Bridge.NET/productiveRage.immutable.extensions.meta.js"))
    .then(() => import(/* webpackChunkName: "bridge.js" */ "Bridge.NET/bridge.js"))
    .then(() => import(/* webpackChunkName: "CRED.Client.js" */ "Bridge.NET/CRED.Client.js"));

    //.then(() =>import(/* webpackChunkName: "i[index].[request]" */ "Bridge.NET/jquery-2.2.4.js"))
// ReSharper restore ExperimentalFeature

//require("external.js");
//require("app.js");

//require("imports-loader?this=>window!../../CRED.Client/bin/Debug/net46/wwwroot/Bridge.NET/bridge.js");
//require("../../CRED.Client/bin/Debug/net46/wwwroot/Bridge.NET/CRED.Client.meta.js");

//import "semantic-ui-less/semantic.less";

//if (module.hot) {
//    module.hot.accept(
//        function () {
//            // Do something with the updated library  module...
//            // Do something with the updated library  module...
//        });
//}

//document.body.appendChild(document.createElement("div"));