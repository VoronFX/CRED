const path = require("path");
const webpack = require("webpack");
//const ExtractTextPlugin = require("extract-text-webpack-plugin");
const merge = require("webpack-merge");
const wwwroot = "wwwroot";

const common = {
    resolve: {
        // For modules referenced with no filename extension, Webpack will consider these extensions
        extensions: [".js", ".ts"],
        modules: [path.resolve(__dirname, wwwroot), "node_modules"],
        alias: {
            '../../theme.config$': path.join(__dirname, wwwroot, "semantic", "theme.config.less")
        }
    },
    output: {
        // ... and emit the built result in this location
        path: path.join(__dirname, wwwroot, "build"),
        publicPath: "/build/",
        filename: "[name]"
    },
    plugins: [
        // Put webpack plugins here if needed, or leave it as an empty array if not
        //new ExtractTextPlugin("styles.css"),
        new webpack.DefinePlugin({
            'process.env': {
                'NODE_ENV': JSON.stringify("production")
            }
        })
    ],
    resolveLoader: {
        modules: [
            "node_modules",
            path.resolve(__dirname, "BuildScripts")
        ]
    }
}

const dev = {
    entry: [
        // The loader will follow all chains of reference from this entry point...
        "main.js"
    ],
    module: {
        loaders: [
            // This example only configures Webpack to load .ts files. You can also drop in loaders
            // for other file types, e.g., .coffee, .sass, .jsx, ...
            //{ test: /\.ts$/, loader: 'ts-loader' }
        ],
        rules: [
            {
                test: /\.css$/,
                use: [
                    "style-loader",
                    "css-loader"
                ]
            },
            {
                test: /\.less/,
                use: [
                    "style-loader",
                    "css-loader" ,
                    "less-loader"
                ]
            },
            {
                test: /\.(png|svg|jpg|gif)$/,
                use: [
                    "file-loader"
                ]
            },
            {
                test: /\.(woff|woff2|eot|ttf|otf)$/,
                use: [
                    "file-loader"
                ]
            },
            {
                test: /\.(csv|tsv)$/,
                use: [
                    "csv-loader"
                ]
            },
            {
                test: /\.xml$/,
                use: [
                    "xml-loader"
                ]
            },
            {
                test: /CRED\.Client\.js$/,
                loader: "regexp-replace-loader",
                options: {
                    match: {
                        pattern: "globals\\.System\\s=\\s\\{\\};"
                    },
                    replaceWith: "$& System = globals.System; "
                }
            }
        ]

    },

};

const cssmap = {
    entry: [ "main.less" ],
    module: {
        rules: [
            {
                test: /main\.less/,
                use: [
                    {
                        loader: "file-loader",
                        options: {
                            name: "CssClassesMap.cs"
                        }
                    },
                    {
                        loader: "csharptask-loader",
                        options: {
                            csharpFilePath: "BuildScripts/ValueMapper.cs",
                            namespace: "CRED.TypedMaps",
                            className: "Styles"
                        }
                    },
                    {
                        loader: "css-loader",
                        options: {
                            url: false
                        }
                    },
                    "less-loader"
                ]
            }
        ]
    }
}


if (process.env.CSSMAP) {
    module.exports = merge(common, cssmap);
} else {
    module.exports = merge(common, dev); 
}