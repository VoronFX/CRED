const CleanWebpackPlugin = require("clean-webpack-plugin"); //installed via npm
const BundleAnalyzerPlugin = require("webpack-bundle-analyzer").BundleAnalyzerPlugin;
//const ExtractTextPlugin = require("extract-text-webpack-plugin");
const path = require("path");
const webpack = require("webpack");
const merge = require("webpack-merge");
const wwwroot = "wwwroot";

const common = {
    resolve: {
        // For modules referenced with no filename extension, Webpack will consider these extensions
        extensions: [".js", ".ts"],
        modules: [
            path.resolve(__dirname, wwwroot),
            path.join("..", "..", "CRED.Client", "bin", "Debug", "net46", "wwwroot"),
            "node_modules"
        ],
        alias: {
            "../../theme.config$": path.join(__dirname, wwwroot, "semantic", "theme.config.less"),
            jquery$: "jquery/src/jquery",
            react$: "react/react",
            "react-dom$": "react-dom/index"
        }
    },
    output: {
        // ... and emit the built result in this location
        path: path.join(__dirname, wwwroot, "build"),
        publicPath: "/build/",
        filename: "[name]",
        chunkFilename: "[name].[id].[query].[chunkhash].js"
    },
    plugins: [
        // Put webpack plugins here if needed, or leave it as an empty array if not
        //new ExtractTextPlugin("styles.css"),
        new CleanWebpackPlugin(["wwwroot/build"]),
        //new webpack.DefinePlugin({
        //    'process.env': {
        //        'NODE_ENV': JSON.stringify("production")
        //    }
        //}),
        new BundleAnalyzerPlugin({
            openAnalyzer: false
        })
    ],
    module: {
        rules: [
            {
                test: /\.js$/,
                loader: "regexp-replace-loader",
                options: {
                    match: {
                        pattern: /([\(\s;,>{\[:!])System(?![\$\w\s\d])/,
                        flags: "g"
                    },
                    replaceWith: `$1window.System`
                }
            },
            {
                test: require.resolve("react"),
                use: [
                    {
                        loader: "expose-loader",
                        options: "React"
                    }
                ]
            },
            {
                test: require.resolve("react-dom"),
                use: [
                    {
                        loader: "expose-loader",
                        options: "ReactDOM"
                    }
                ]
            },
            {
                test: require.resolve("jquery"),
                use: [
                    {
                        loader: "expose-loader",
                        options: "jQuery"
                    }, {
                        loader: "expose-loader",
                        options: "$"
                    }
                ]
            }
        ]
    },
    resolveLoader: {
        modules: [
            "node_modules",
            path.resolve(__dirname, "BuildScripts")
        ]
    }
};

const dev = {
    entry: {
        // The loader will follow all chains of reference from this entry point...
        "main.bundle.js": "main.js"
    },
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
                test: /\.less$/,
                use: [
                    "style-loader",
                    "css-loader",
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
            //{
            //    test: /\.js$/,
            //    use: ["source-map-loader"],
            //    enforce: "pre"
            //}
        ]

    },
    //devtool: "eval-source-map",
    plugins: [
        //new webpack.SourceMapDevToolPlugin({
        //    exclude: /CRED\.Client/
        //}),
        //new webpack.HotModuleReplacementPlugin(),
        //new webpack.NamedModulesPlugin(),
        new webpack.BannerPlugin({
            banner: "name:[name], query:[query], file:[file], hash:[hash], chunkhash:[chunkhash],  filebase:[filebase]",
            entryOnly: false
        })
    ]
};

const cssmap = {
    entry: ["main.less"],
    module: {
        rules: [
            {
                test: /main\.less/,
                use: [
                    {
                        loader: "file-loader",
                        options: {
                            name: "CssClassesMap.Generated.cs"
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
};

const mergset = process.env.CSSMAP ? cssmap : dev;

const merged = merge(common, mergset);

//console.log(merged);

module.exports = merged;