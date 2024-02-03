package cmd

import (
	"context"
	"encoding/base64"
	"os"

	"github.com/spf13/cobra"
)

var j2pCmd = &cobra.Command{
	Use:   "j2p  --payload=payload --desc=desc --name=name --sn=sn",
	Short: "JSON to ProtoBuf",
	Long:  `Converts JSON to ProtoBuf`,
	Run: func(cmd *cobra.Command, args []string) {
		handleJ2P(cmd.Context())
	},
}

func init() {
	j2pCmd.Flags().StringVar(&payload, "payload", "", "json payload")
	j2pCmd.Flags().StringVar(&descriptor, "desc", "", "proto-buf descriptor")
	j2pCmd.Flags().StringVar(&name, "name", "", "name of the message")
	j2pCmd.Flags().StringVar(&sname, "sn", "", "path to proto-buf file")
	j2pCmd.MarkFlagRequired("payload")
	j2pCmd.MarkFlagRequired("desc")
	j2pCmd.MarkFlagRequired("name")
	j2pCmd.MarkFlagRequired("sn")
	rootCmd.AddCommand(j2pCmd)
}

func handleJ2P(context context.Context) {
	jbytes, err := base64.StdEncoding.DecodeString(payload)
	if err != nil {
		terminate(err)
	}
	desc, err := compileDescriptor(descriptor, name, sname)
	if err != nil {
		terminate(err)
	}
	pb, err := jsonToProto(jbytes, desc)
	if err != nil {
		terminate(err)
	}
	p64 := base64.StdEncoding.EncodeToString(pb)
	os.Stdout.WriteString(p64)
}
